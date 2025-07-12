#define MyAppName "TiContent"
#define MyAppVersion "1.4"
#define MyAppPublisher "the_timick"
#define MyAppExeName "TiContent.UI.WinUI.exe"

[Setup]
AppId={{C4889E55-34D8-41BF-AFF5-FEE286DC67FF}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} v.{#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf64 }\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputBaseFilename=setup
SolidCompression=yes
VersionInfoVersion = 4.5.0
DisableReadyPage = Yes
DirExistsWarning = No
SetupIconFile = _installer\icon.ico
UninstallFilesDir = {app}\Uninstall
Compression = lzma2/Max
DiskSpanning = false
PrivilegesRequired = admin

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "input\*.*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: FilesAndOrDirs; Name: {app}

