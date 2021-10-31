# Cooking

Application to manage home recipies collection and create week schedules based on this collection.

## Features

- Recipe library creation and management
- Creation of week schedule based on user-defined filters
- Make a shopping list based on a schedule
- Different types of applications with single infrastructure: WPF, ASP.NET Core, Avalonia

## Contributing

- For automatic xaml format please install pre-hook via script in githooks\install.ps1

## Used libraries

### WPF

- Prism: Navigation, Dependency injection
- Automapper (with Automapper.Collection): mapping between application levels
- Fody: code generation
-- ConfigureAwait.Fody: Automatic use of ConfigureAwait(false)
-- NullGuard.Fody: Automatic check for method parameters to be not null
-- Validar.Fody: Validation injection
- FluentValidation: Validation rules
- gong-wpf-gragdrop: Drag and drop handling via MVVM
- Mahapps.Metro: UI components (dialogs and controls)
- Mahapps.Metro.IconPacks.Modern: Application's icons
- MaterialDesignColors: UI framework
- MaterialDesignThemes: themes for UI framework
- PhotoSause.MagicScaler: Auto resize of user's images
- Plafi: Customizable on-the-fly check of user filters
- Serilog: Logging
- VirtualizingWrapPanel: Virtualization containers
- WPFLocalizeExtention: Localization
- SmartFormat.NET: string pluralization
- WPF.Commands: commands library

### Avalonia

- Avalonia: Platform

### Database layer

- Entity Framework Core: ORM
- Sqlite: Database

### Tests
- Moq: mocking
- xUnit: Test framework
- FluentAssertions: more readable assertions

### ASP.NET Core
- RtfPipe: Converting from RTF format to html

### Static code analisys

- Roslynator
- StyleCop

### VS Extentions

- XamlStyler
- Fine Code Coverage