<div align="center">
  <p>
    <a href="CONTRIBUTING.md">Bahasa Indonesia</a> · <b>English</b>
  </p>
</div>

# Contributing to Sharphy

Thank you for your interest in contributing to **Sharphy**! This project welcomes anyone looking to expand runtime diagnostics, CLR forensics, and MCP server integrations within the .NET ecosystem.

---

## Code of Conduct

Please be respectful, collaborative, and constructive in all issues, pull requests, and discussions.

---

## Development Setup

### Prerequisites
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download) (multi-targeting covers .NET 8, 9, and 10).
- Git.

### Building the Project
```bash
dotnet restore
dotnet build
```

### Running Tests
```bash
dotnet test
```

---

## Pull Request Guidelines

1. **Fork the Repository:** Create a dedicated feature branch from `main`:
   ```bash
   git checkout -b feature/your-feature-name
   ```
2. **Include Tests:** Add relevant unit tests or test scenarios in `tests/Sharphy.Tests`.
3. **Follow Code Standards:**
   - Write clean, modern C# and respect nullable reference types.
   - Ensure the solution builds cleanly across all target frameworks without introducing new warnings.
4. **Submit PR:** Open a pull request targeting `main` with a descriptive summary of your changes.
