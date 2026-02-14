## 1. Update data directory in Database.fs

- [x] 1.1 Change `dataDir` from `AppContext.BaseDirectory/data` to `~/.church-attendance/data/` using `Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)`
- [x] 1.2 Add migration hint: on startup, if old location has data but new location is empty, print a console message with copy instructions

## 2. Clean up project files

- [x] 2.1 Remove `<Content Update="data\**" CopyToOutputDirectory="PreserveNewest" />` from `.fsproj`
- [x] 2.2 Delete seed data files from `ChurchAttendance/data/` directory

## 3. Verify

- [x] 3.1 Run the app with `dotnet run` and confirm it creates `~/.church-attendance/data/` and operates normally
