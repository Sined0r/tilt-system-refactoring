# Tilt System Refactoring

## Project Goal
Refactor a legacy Unity MonoBehaviour class into a clean, testable C# domain model.

The original implementation mixed UI logic, input handling, physics calculations, and state management inside a single 300+ line class.  
This project extracts pure business logic into isolated domain components independent from Unity.

---

## Architecture Overview

The refactored system follows basic Domain-Driven Design principles:

- Screw — Entity representing a height-adjustable support with constraints
- Platform — Aggregate root encapsulating three screws and tilt calculation workflow
- TiltCalculator — Domain service responsible for mathematical tilt computation
- Tests — Basic behavioral verification of tilt scenarios

All core logic is fully decoupled from Unity-specific types (MonoBehaviour, Vector3, etc.).

---

## Refactoring Objectives

- Separate domain logic from framework code
- Improve readability and maintainability
- Enable testability
- Apply Single Responsibility Principle
- Introduce explicit domain model

---

## Current Status

- Legacy implementation preserved in original/
- Domain logic extracted into src/
- Basic behavioral tests implemented in tests/

Further improvements may include:
- Proper test framework integration (xUnit/NUnit)
- CI pipeline setup
- Extended validation scenarios

---

## Tech Stack

- C#
- .NET
- Unity (original source context only)