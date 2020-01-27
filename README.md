# Cooking

Application to manage home recipies collection and create week schedules based on this collection.

## Features

- Recipe library creation and management
- Creation of week schedule based on user-defined filters
- Make a shopping list based on a schedule
- Different types of applications with single infrastructure: WPF, ASP.NET Core, Avalonia
## Used libraries

### WPF

- Prism: Navigation, Dependency injection
- Automapper (with Automapper.Collection): mapping between application levels
- Fody: code generation
-- Bindables.Fody: Simpyfying DependencyProperty definition
-- ConfigureAwait.Fody: Automatic use of ConfigureAwait(false)
-- NullGuard.Fody: Automatic check for method parameters to be not null
-- Validar.Fody: Validation injection
- ExCeed Extended WFP Toolkit: Colorpicker, RichTextBox
- FluentValidation: Validation rules
- gong-wpf-gragdrop: Drag and drop handling via MVVM
- Mahapps.Metro: UI framework
- Mahapps.Metro.IconPacks.Modern: Application's icons
- PhotoSause.MagicScaler: Auto resize of user's images
- Plafi: Customizable on-the-fly check of user filters
- ReportGenerator: Code coverage reports generator
- Serilog: Logging
- VirtualizingWrapPanel: Virtualization containers
- WPFLocalizeExtention: Localization

### Database layer

- Entity Framework Core: ORM
- Sqlite: Database
- 

### Tests
- Moq: mocking
- MS Test Framework: Test framework

### ASP.NET Core
- RtfPipe: Converting from RTF format to html
