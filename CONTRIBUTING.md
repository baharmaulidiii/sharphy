# Contributing to Sharphy

Thank you for your interest in contributing to **Sharphy**!

## Code of Conduct
Please be respectful and constructive in all discussions, pull requests, and issues.

## Development Setup

### Prerequisites
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download) (multi-targeting covers .NET 8, 9, 10).
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

## Pull Request Guidelines
1. Fork the repo and create a feature branch (`git checkout -b feature/my-new-tool`).
2. Add appropriate unit tests in `tests/Sharphy.Tests`.
3. Ensure all projects compile cleanly without warnings/errors across targeted frameworks.
4. Submit your pull request with a descriptive title and summary of changes.
