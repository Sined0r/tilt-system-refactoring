# Tilt System Refactoring

## Project Goal
Demonstrate refactoring of a legacy Unity MonoBehaviour class into clean, testable C# domain architecture.

The original implementation contained UI logic, input handling, and tilt calculations in a single 300+ line class.  
This project extracts business logic into isolated, testable components.

## Refactoring Objectives
- Separate domain logic from Unity-specific code
- Improve readability and maintainability
- Enable unit testing
- Apply Single Responsibility Principle

## Current Status
Step 1: Extract tilt calculation into standalone class.

## Tech Stack
- C#
- .NET
- Unity (original source context)