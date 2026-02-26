using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leica : MonoBehaviour
{
    [Header("Surveyor Settings")]
    public float derectionRadius = 5f;
    public GameObject Surveyor;

    [Header("UI Settings")]
    public GameObject _AimUI;
    public GameObject _LeicaUI;

    [Header("Screws")]
    public Transform screw1;
    public Transform screw2;
    public Transform screw3;

    [Header("Screw Sprites")]
    [SerializeField] private RectTransform screw1UI;
    [SerializeField] private RectTransform screw2UI;
    [SerializeField] private RectTransform screw3UI;

    // В самом начале класса добавим поля для хранения начальных позиций спрайтов
    private Vector3 initialScrew1UIPosition;
    private Vector3 initialScrew2UIPosition;
    private Vector3 initialScrew3UIPosition;

    [Header("Level UI")]
    [SerializeField] private LevelUI levelUI;
    [SerializeField] public Camera _Lcamera;
    [SerializeField] private LevelUI secondLevelUI;

    private List<Transform> selectedScrews = new List<Transform>();
    private const int maxSelectedScrews = 2; // Максимум два винта

    private bool _nearby = false;
    private bool _inLeica = false;

    // Параметры работы винтов
    private float screw1Height = 0f;
    private float screw2Height = 0f;
    private float screw3Height = 0f;

    [Header("Other Settings")]
    public float adjustmentSpeed = 0.00013f;  // Скорость регулировки
    public float maxTiltAngle = 2.0f; // Ограничение отклонения платформы
    public float screwRadius = 0.1f;
    public Transform platform;
    private bool isPaused = false;
    public enum LeicaSubmode { None, Eyepiece, Sight, Panel }
    private LeicaSubmode currentSubmode = LeicaSubmode.None;
    public PlayerStateManager playerStateManager;


    private void Awake()
    {
        _LeicaUI.SetActive(false);
        _AimUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _Lcamera.enabled = false;
    }

    private void Start()
    {
        // Сохраняем начальные позиции спрайтов
        initialScrew1UIPosition = screw1UI.localPosition;
        initialScrew2UIPosition = screw2UI.localPosition;
        initialScrew3UIPosition = screw3UI.localPosition;
    }

    private void Update()
    {
        if (isPaused) return;

        if (Input.GetMouseButtonDown(0) && !_inLeica)
        {
            TryActivateLeicaWithRaycast();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && _inLeica)
        {
            if (currentSubmode != LeicaSubmode.None)
            {
                ExitCurrentSubmode(); // выходим из подрежима
            }
            else
            {
                ToggleLeicaMode(); // выходим из Leica вообще
            }
        }

        if (_inLeica)
        {
            // Прокрутка колеса мыши для изменения высоты винтов
            float scrollDelta = Input.mouseScrollDelta.y;
            if (scrollDelta != 0)
            {
                var highlightedScrews = GetHighlightedScrews();
                if (highlightedScrews.Count > 0)
                {
                    AdjustHighlightedScrews(highlightedScrews, scrollDelta);
                }
            }
        }
        else
        {
            CheckPlayerDerection();
        }

        Vector3 tilt = CalculateTilt();
        platform.localRotation = Quaternion.Euler(Vector3.ClampMagnitude(tilt, maxTiltAngle));
        levelUI.UpdateLevelUI(new Vector2(tilt.x, tilt.z));
        if (secondLevelUI != null)
            secondLevelUI.UpdateLevelUI(new Vector2(tilt.x, tilt.z));
    }

    private void TryActivateLeicaWithRaycast()
    {

        if (Camera.main == null)
        {
            //Debug.LogWarning("Main camera is not assigned");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // Проверка, попали ли в объект с этим скриптом
            if (hit.transform == this.transform ||
                (hit.transform.IsChildOf(this.transform) && this.transform != null))
            {
                float distance = Vector3.Distance(hit.point, transform.position);
                if (distance <= derectionRadius)
                {
                    _nearby = true;
                    ToggleLeicaMode();
                }
            }
        }
        else
        {
            // Добавляем else для завершения логики
            _nearby = false; // Сбрасываем флаг, если луч никуда не попал
            Debug.Log("Raycast didn't hit any object");
        }
    }

    /*private void TryActivateLeicaWithRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // Проверка, попали ли в объект с этим скриптом
            if (hit.transform == this.transform || hit.transform.IsChildOf(this.transform))
            {
                float distance = Vector3.Distance(hit.point, transform.position);
                if (distance <= derectionRadius)
                {
                    _nearby = true;
                    ToggleLeicaMode();
                }
            }
        }
    }*/


    private void ToggleLeicaMode()
    {
        _inLeica = !_inLeica;

        if (playerStateManager != null)
            playerStateManager.SetSpecialMode(_inLeica);

        if (_inLeica && _nearby)
        {
            Surveyor?.SetActive(false);
            _LeicaUI?.SetActive(true);
            _AimUI?.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _Lcamera.enabled = true;

            InventoryManager.Instance?.LockInventory(true);
        }
        else
        {
            Surveyor?.SetActive(true);
            _LeicaUI?.SetActive(false);
            _AimUI?.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _Lcamera.enabled = false;

            InventoryManager.Instance?.LockInventory(false);
            InventoryManager.Instance?.ForceInventoryReset();

            FindObjectOfType<PauseManager>()?.IgnoreEscapeThisFrame();
        }
    }

    private List<Transform> GetHighlightedScrews()
    {
        List<Transform> highlighted = new List<Transform>();

        if (screw1.GetComponent<Outline>()?.enabled == true) highlighted.Add(screw1);
        if (screw2.GetComponent<Outline>()?.enabled == true) highlighted.Add(screw2);
        if (screw3.GetComponent<Outline>()?.enabled == true) highlighted.Add(screw3);

        return highlighted;
    }

    public void HighlightScrew(Transform screw, bool highlight)
    {
        var outline = screw.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = highlight;
        }
    }

    private void AdjustHighlightedScrews(List<Transform> screws, float scrollDelta)
    {
        float adjustment = scrollDelta * adjustmentSpeed;

        if (screws.Count == 1)
        {
            AdjustScrewHeight(screws[0], adjustment);
        }
        else if (screws.Count == 2)
        {
            AdjustScrewHeight(screws[0], adjustment);
            AdjustScrewHeight(screws[1], -adjustment);
        }

        UpdateScrewPositions();
    }

    private void AdjustScrewHeight(Transform screw, float delta)
    {
        if (screw == screw1) screw1Height = Mathf.Clamp(screw1Height + delta, -0.005f, 0.005f);
        else if (screw == screw2) screw2Height = Mathf.Clamp(screw2Height + delta, -0.005f, 0.005f);
        else if (screw == screw3) screw3Height = Mathf.Clamp(screw3Height + delta, -0.005f, 0.005f);
    }


    private void UpdateScrewPositions()
    {
        // Ограничения высоты винтов
        float minHeight = -0.005f;
        float maxHeight = 0.005f;

        // Применяем ограничения и обновляем позиции винтов
        screw1Height = Mathf.Clamp(screw1Height, minHeight, maxHeight);
        screw2Height = Mathf.Clamp(screw2Height, minHeight, maxHeight);
        screw3Height = Mathf.Clamp(screw3Height, minHeight, maxHeight);

        screw1.localPosition = new Vector3(screw1.localPosition.x, screw1Height, screw1.localPosition.z);
        screw2.localPosition = new Vector3(screw2.localPosition.x, screw2Height, screw2.localPosition.z);
        screw3.localPosition = new Vector3(screw3.localPosition.x, screw3Height, screw3.localPosition.z);

        // Ограничения и обновление позиций спрайтов UI
        float uiMinHeight = -25f; // Минимальное изменение относительно начальной позиции
        float uiMaxHeight = 25f;  // Максимальное изменение относительно начальной позиции

        float normalizedScrew1 = Mathf.InverseLerp(minHeight, maxHeight, screw1Height);
        float normalizedScrew2 = Mathf.InverseLerp(minHeight, maxHeight, screw2Height);
        float normalizedScrew3 = Mathf.InverseLerp(minHeight, maxHeight, screw3Height);

        screw1UI.localPosition = initialScrew1UIPosition + new Vector3(0, Mathf.Lerp(uiMinHeight, uiMaxHeight, normalizedScrew1), 0);
        screw2UI.localPosition = initialScrew2UIPosition + new Vector3(0, Mathf.Lerp(uiMinHeight, uiMaxHeight, normalizedScrew2), 0);
        screw3UI.localPosition = initialScrew3UIPosition + new Vector3(0, Mathf.Lerp(uiMinHeight, uiMaxHeight, normalizedScrew3), 0);

        Debug.Log($"Screw positions updated: Screw1({screw1Height}), Screw2({screw2Height}), Screw3({screw3Height})");
    }

    private Vector3 CalculateTilt()
    {
        float deltaX = (screw2Height - screw1Height) / (2 * screwRadius);
        float deltaY = (screw3Height - (screw1Height + screw2Height) / 2) / (Mathf.Sqrt(3) * screwRadius);

        float tiltX = Mathf.Rad2Deg * Mathf.Atan(deltaX);
        float tiltY = Mathf.Rad2Deg * Mathf.Atan(deltaY);

        return new Vector3(tiltX, 0, tiltY);
    }

    private void CheckPlayerDerection()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, derectionRadius);
        _nearby = false;

        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponent<FirstPersonMovement>() != null)
            {
                _nearby = true;
                break;
            }
        }
    }

    public void EnterSubmode(LeicaSubmode mode)
    {
        currentSubmode = mode;
    }

    public void ExitCurrentSubmode()
    {
        switch (currentSubmode)
        {
            case LeicaSubmode.Eyepiece:
            case LeicaSubmode.Sight:
            case LeicaSubmode.Panel:
                if (TryGetComponent(out ViewSwitcher viewSwitcher))
                    viewSwitcher.SwitchToMainView();
                break;
        }

        currentSubmode = LeicaSubmode.None;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, derectionRadius);
    }

    public void SetPauseState(bool paused)
    {
        isPaused = paused;
    }
}
