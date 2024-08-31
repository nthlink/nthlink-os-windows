obfuscar.console .\ReleaseWorkSpace\obfuscarconfig_x64.xml
obfuscar.console .\ReleaseWorkSpace\obfuscarconfig_x86.xml

xcopy /y /f .\ReleaseWorkSpace\ReleaseData\Plugins\x64\Confused\nthLink.Plugins.Leaf.dll .\ReleaseWorkSpace\ReleaseData\x64\Plugins\
xcopy /y /f .\ReleaseWorkSpace\ReleaseData\Plugins\x64\Confused\outline-sdk-windows.dll .\ReleaseWorkSpace\ReleaseData\x64\Plugins\

xcopy /y /f .\ReleaseWorkSpace\ReleaseData\Plugins\x64\nthLink.Plugins.Leaf.deps.json .\ReleaseWorkSpace\ReleaseData\x64\Plugins\
xcopy /y /f .\ReleaseWorkSpace\ReleaseData\Plugins\x64\outline-sdk-windows.deps.json .\ReleaseWorkSpace\ReleaseData\x64\Plugins\

xcopy /y /f .\ReleaseWorkSpace\ReleaseData\Plugins\x64\Confused\nthLink.Header.dll .\ReleaseWorkSpace\ReleaseData\x64\
xcopy /y /f .\ReleaseWorkSpace\ReleaseData\Plugins\x64\Confused\nthLink.SDK.dll .\ReleaseWorkSpace\ReleaseData\x64\

xcopy /y /f .\ReleaseWorkSpace\ReleaseData\Plugins\x86\Confused\nthLink.Plugins.Leaf.dll .\ReleaseWorkSpace\ReleaseData\x86\Plugins\
xcopy /y /f .\ReleaseWorkSpace\ReleaseData\Plugins\x86\Confused\outline-sdk-windows.dll .\ReleaseWorkSpace\ReleaseData\x86\Plugins\

xcopy /y /f .\ReleaseWorkSpace\ReleaseData\Plugins\x86\nthLink.Plugins.Leaf.deps.json .\ReleaseWorkSpace\ReleaseData\x86\Plugins\
xcopy /y /f .\ReleaseWorkSpace\ReleaseData\Plugins\x86\outline-sdk-windows.deps.json .\ReleaseWorkSpace\ReleaseData\x86\Plugins\

xcopy /y /f .\ReleaseWorkSpace\ReleaseData\Plugins\x86\Confused\nthLink.Header.dll .\ReleaseWorkSpace\ReleaseData\x86\
xcopy /y /f .\ReleaseWorkSpace\ReleaseData\Plugins\x86\Confused\nthLink.SDK.dll .\ReleaseWorkSpace\ReleaseData\x86\
PAUSE