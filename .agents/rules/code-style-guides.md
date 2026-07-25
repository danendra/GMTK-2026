---
trigger: always_on
---

Coding Rules

1. Use the namespace `GMTK` for every C# script.
2. Follow modular programming principles:
   - One script should be responsible for one feature only.
   - Avoid combining unrelated responsibilities into a single script.
3. Prefer composition over monolithic classes.
4. Use Object-Oriented Programming (OOP) principles whenever appropriate.
5. Use inheritance only when there is a clear parent-child relationship.
6. Use interfaces to define shared behaviors instead of tightly coupling implementations.
7. Naming conventions:
   - Variables: camelCase
   - Methods/Functions: PascalCase
   - Classes: PascalCase
   - Interfaces: Prefix with `I` (e.g., `IDamageGive`, `IDamageTake`, `IHealth`)
   - Enums: ALL_CAPS
8. Feature scripts should use descriptive names, for example:
   - MoveController
   - InputController
   - ShootController
9. Child classes should extend base functionality clearly, for example:
   - ZigZagMoveController : MoveController
10. Follow the file naming convention:
    - `*Manager` → Coordinate or manage multiple systems/features.
    - `*Controller` → Handle a single gameplay feature or behaviour.
    - `*Data` → Store data, configurations, or ScriptableObjects.
11. Keep scripts reusable, maintainable, and loosely coupled.
12. Avoid duplicated code. Extract shared logic into base classes, utility classes, or interfaces when appropriate.
13. Keep each class focused on a single responsibility (Single Responsibility Principle).
14. Write clean, readable, and scalable code that is easy to extend for future features.