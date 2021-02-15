Import-LocalizedData -BindingVariable msgTable

$IsContinuing = Read-Host $msgTable.q

if ($IsContinuing -ne "y")
{
    exit
}

dotnet tool install --global XamlStyler.Console
cmd /c mklink /h ..\.git\hooks\pre-commit ..\githooks\pre-commit