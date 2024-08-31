	Unicode True
	;--------------------------------
	!include LogicLib.nsh
	!include FileFunc.nsh
	
	;Include Modern UI
	!include MUI2.nsh
	
	!include WinMessages.nsh
	;--------------------------------
	
	!define Product_Name "nthLink"
	!define License_Folder "License"
	!define Resource_Floder "Resource"
	!define Source_File_Path "ReleaseData\${TargetPlatform}"
	!define Icon_FileName "icon.ico"
	
	!define MUI_ICON "${Resource_Floder}\${Icon_FileName}"
	!define MUI_UNICON "${Resource_Floder}\${Icon_FileName}"
		
	!define MUI_ABORTWARNING
	
	!insertmacro GetSize
	
	!insertmacro MUI_PAGE_WELCOME
	;!insertmacro MUI_PAGE_LICENSE "${License_Folder}\${License_FileName}"
	!insertmacro MUI_PAGE_COMPONENTS
	!insertmacro MUI_PAGE_DIRECTORY
	!insertmacro MUI_PAGE_INSTFILES
	
	!insertmacro MUI_PAGE_FINISH
	
	!insertmacro MUI_UNPAGE_WELCOME 
	;!insertmacro MUI_UNPAGE_CONFIRM
	!insertmacro MUI_UNPAGE_INSTFILES
	!insertmacro MUI_UNPAGE_FINISH
	;--------------------------------
	!insertmacro MUI_LANGUAGE "English"
	; !insertmacro MUI_LANGUAGE "SimpChinese"
	; !insertmacro MUI_LANGUAGE "TradChinese"
	
	; The name of the installer
	Name "${Product_Name} Installer"	
	
	; The BrandingText on install MessageBox
	BrandingText "${Product_Name}"	
	
	; The file Properties to write	
	;VIAddVersionKey CompanyName "${Company_Name}"
	VIAddVersionKey FileDescription "${Product_Name} Installation"
	VIAddVersionKey FileVersion "${Version}"
	VIAddVersionKey ProductName "${Product_Name}"
	VIProductVersion "${Version}"
	VIAddVersionKey ProductVersion "${Version}"
	VIAddVersionKey LegalCopyright "Copyright(c) 2024 ${Product_Name} Corp.All rights"
		
	Function .onInit
		${If} ${TargetPlatform} == "x64"
			StrCpy $INSTDIR "$PROGRAMFILES64\${Product_Name}"
		${Else}
			StrCpy $INSTDIR "$PROGRAMFILES\${Product_Name}"
		${EndIf}
	FunctionEnd
	
	OutFile "${Product_Name}_Installer_${TargetPlatform}_${Version}.exe"
	
	RequestExecutionLevel admin
	ManifestSupportedOS all
	AutoCloseWindow false
	ShowInstDetails hide
	XPStyle on
	
	!define VCPulsPuls "VC_redist.${TargetPlatform}.exe"	
	!define Net6 "windowsdesktop-runtime-win-${TargetPlatform}.exe"
	
		
	Section "Visual C++ Runtime"
	SectionIn RO
	SetOutPath "$INSTDIR"
	File "${Resource_Floder}\${TargetPlatform}\${VCPulsPuls}"
	ExecWait "$INSTDIR\${VCPulsPuls} /install /passive"
	Delete "$INSTDIR\${VCPulsPuls}"
	SectionEnd	
	
	Section ".Net 6 Desktop Runtime"
	SectionIn RO
	SetOutPath "$INSTDIR"
	File "${Resource_Floder}\${TargetPlatform}\${Net6}"
	ExecWait "$INSTDIR\${Net6} /install /quiet /norestart"
	Delete "$INSTDIR\${Net6}"
	SectionEnd
	
	Section "Install"
	  FindWindow $0 "" "nthLink"
	  SendMessage $0 ${WM_CLOSE} 0 0
	  Sleep 1500
	; Clear Folder
	Delete "$INSTDIR\*.*"
	
	RMDir /r "$INSTDIR\"
	
	; Precision Setup, update function provider
	CreateDirectory $INSTDIR
	CreateDirectory $INSTDIR\locales
	CreateDirectory $INSTDIR\Plugins

	SetOutPath $INSTDIR\locales
	File ${Source_File_Path}\locales\*.*
	
	SetOutPath $INSTDIR
	; Put file there
	File ${Source_File_Path}\*.bin
	File ${Source_File_Path}\*.json
	File ${Source_File_Path}\*.sys
	File ${Source_File_Path}\*.exe
	File ${Source_File_Path}\*.dll
	File ${Source_File_Path}\*.pak
	File ${Source_File_Path}\*.dat
	
	SetOutPath $INSTDIR\Plugins
	File ${Source_File_Path}\Plugins\*.dll
	File ${Source_File_Path}\Plugins\*.json
	
	SetOutPath $INSTDIR
	;ExecShellWait "" "$INSTDIR\DataProtector.exe --key:apikey --value:vLqNzoHPd25qsu/eSh4vg6jdbRm9rOyLBqp1yswoKQfjyqBZk3hDGHlgD7W86nMPX7ZNaQqBAtRMMA9m3+FTeqB4uV78GOSAXsoyNsI29SvOzCXHLq2yUgw8PSbhZSzlcu7ePPSkDMFr6qBQeMnbPtkUiDzOJVwkLoptpF3wA7UoIPpM95i2Tu1cNC1sD2I3tVokNmO9MEVdfdv1A0PJ8lZcwRziPBWqJZ4ow3uV+CBVKErGPxVHWj5U1Ert69FrdJ8oEcCGX8EbM3XfycVSdCOy1UHS+Kr4nhaugtAaLJa2m7bROiw9jXAgxLD1EWao2q4KPEF8nwHgxupYaNd0wiIYD1BJ61lBZGdSDqhb6LSbtgK3EmBi4lBTvwFvz8fOT90k9ntYVNvnRe7ttJdRsPW+SSCYNp/E52soPo0DSNedJQiSC8JyvakAFKIYgCF4KEFcZHlymNw4ctQ5OiMN1wBkXvzJlIgdrK1Me7IJOIfRl/2ra6ilD6DPaFHLVElUlaojZQGReWtikpSCjlEuCx0lkQSx9YilEBPFuDgsHyThKSlADGmKH0GVg/cNSwiwJv4SAZKjRw9GNR3ywzzfcASv8Nk3GctVEJWhXHC9yatLsDja7uzzRYwaPn0K+yjtKd3G8g3PAfIw6APAILc19AXvBxYcbwr+IYNQAqmfO0SRVK6SjCOnDYa15WprE5tbAukCKO3A6zy0QF0pIT52/gG8HhQ4MvKIPEjlMDK+I0NEFF1Ua3ulliLDNsG6jXo6FeUCF9gJ15viWtLkkrdSxQCN6dWG4ieSsETy9BP/Yf2aTgL93GSn1bnrgTgk4z1jiI1jDNZ9zOuYxcFQ9dnFyRJRhuEwayRpfQvqUqvbQ6lGQsj/dLbMeBfkFP1+VSkOtju4Nej+57suEOWPu+AuT3p35/dLddpsnDTCKrcXF5sCt1m4kkwdwNbFFcNN1+lpgv5SJU6/FLkR66jz7fCfVmDFN3b971dp5jV1Qju4j1q77Wy+JJuQn4mdCJ+vGTYCKsiPapqBQkvVaLfyREnihRI4gQBfKp/gQ7lFycMfLXnzfNjjxbGuaZaZmrIhLAgSktjTNTUTFfXRztRR21Qyfe+cKVyeiTdw69zbzjgJIyh5h4jTrWxZEpBwt3rfC2+WXODaeMPKIQTFw5JWQnwJLcrQ9guTQbNpwKipUvzVzi1kVogOekgBjzYC3jJKidWxpgRUMVOuLCjTME8qfYuSAA/AlMT40qN9FlW8eP4hUP82eoxCkQ+hkytgcAwmaxO/itmyZWYmcuqxM62ThX+lbbflNeGLCihP5Qb3j7MmeYw=" SW_HIDE
	ExecWait "$INSTDIR\DataProtector.exe --key:apikey --value:vLqNzoHPd25qsu/eSh4vg6jdbRm9rOyLBqp1yswoKQfjyqBZk3hDGHlgD7W86nMPX7ZNaQqBAtRMMA9m3+FTeqB4uV78GOSAXsoyNsI29SvOzCXHLq2yUgw8PSbhZSzlcu7ePPSkDMFr6qBQeMnbPtkUiDzOJVwkLoptpF3wA7UoIPpM95i2Tu1cNC1sD2I3tVokNmO9MEVdfdv1A0PJ8lZcwRziPBWqJZ4ow3uV+CBVKErGPxVHWj5U1Ert69FrdJ8oEcCGX8EbM3XfycVSdCOy1UHS+Kr4nhaugtAaLJa2m7bROiw9jXAgxLD1EWao2q4KPEF8nwHgxupYaNd0wiIYD1BJ61lBZGdSDqhb6LSbtgK3EmBi4lBTvwFvz8fOT90k9ntYVNvnRe7ttJdRsPW+SSCYNp/E52soPo0DSNedJQiSC8JyvakAFKIYgCF4KEFcZHlymNw4ctQ5OiMN1wBkXvzJlIgdrK1Me7IJOIfRl/2ra6ilD6DPaFHLVElUlaojZQGReWtikpSCjlEuCx0lkQSx9YilEBPFuDgsHyThKSlADGmKH0GVg/cNSwiwJv4SAZKjRw9GNR3ywzzfcASv8Nk3GctVEJWhXHC9yatLsDja7uzzRYwaPn0K+yjtKd3G8g3PAfIw6APAILc19AXvBxYcbwr+IYNQAqmfO0SRVK6SjCOnDYa15WprE5tbAukCKO3A6zy0QF0pIT52/gG8HhQ4MvKIPEjlMDK+I0NEFF1Ua3ulliLDNsG6jXo6FeUCF9gJ15viWtLkkrdSxQCN6dWG4ieSsETy9BP/Yf2aTgL93GSn1bnrgTgk4z1jiI1jDNZ9zOuYxcFQ9dnFyRJRhuEwayRpfQvqUqvbQ6lGQsj/dLbMeBfkFP1+VSkOtju4Nej+57suEOWPu+AuT3p35/dLddpsnDTCKrcXF5sCt1m4kkwdwNbFFcNN1+lpgv5SJU6/FLkR66jz7fCfVmDFN3b971dp5jV1Qju4j1q77Wy+JJuQn4mdCJ+vGTYCKsiPapqBQkvVaLfyREnihRI4gQBfKp/gQ7lFycMfLXnzfNjjxbGuaZaZmrIhLAgSktjTNTUTFfXRztRR21Qyfe+cKVyeiTdw69zbzjgJIyh5h4jTrWxZEpBwt3rfC2+WXODaeMPKIQTFw5JWQnwJLcrQ9guTQbNpwKipUvzVzi1kVogOekgBjzYC3jJKidWxpgRUMVOuLCjTME8qfYuSAA/AlMT40qN9FlW8eP4hUP82eoxCkQ+hkytgcAwmaxO/itmyZWYmcuqxM62ThX+lbbflNeGLCihP5Qb3j7MmeYw="
	Delete $INSTDIR\DataProtector.exe
	Delete $INSTDIR\DataProtector.dll
	Delete $INSTDIR\DataProtector.deps.json
	Delete $INSTDIR\DataProtector.runtimeconfig.json
		
    ${If} ${TargetPlatform} == "x64"
        SetRegView 64
    ${Else}
        SetRegView 32
    ${EndIf}
	
	; Write the installation path into the registry
	WriteRegStr HKLM "SOFTWARE\${Product_Name}" "InstallDir" "$INSTDIR"
	WriteRegStr HKLM "SOFTWARE\${Product_Name}" "PolicyAgreed" "false"
	; Write the uninstall keys for Windows
	WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${Product_Name}" "DisplayName" "${Product_Name}"
	
	WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${Product_Name}" "DisplayIcon" "$INSTDIR\${Product_Name}.Wpf.exe"
	
	WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${Product_Name}" "Publisher" "${Product_Name} Corporation"
	
	;WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${Product_Name}" "HelpLink" "http://www.Teon.com"
	
	;WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${Product_Name}" "URLInfoAbout" "http://www.Teon.com"
	
	WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${Product_Name}" "UninstallString" "$INSTDIR\uninstall.exe"
	
	WriteRegDWORD HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${Product_Name}" "NoModify" 1
	
	WriteRegDWORD HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${Product_Name}" "NoRepair" 1
	
	WriteUninstaller "uninstall.exe"
		
	WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${Product_Name}" "DisplayVersion""${Version}"	
	
	${GetSize} "$INSTDIR" "/S=0:100000000K" $R2 $R3 $R4
	
	WriteRegDWORD HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${Product_Name}" "EstimatedSize""$R2"
	
	SetShellVarContext all
	
	; Start Menu Shortcuts	
	CreateDirectory "$SMPROGRAMS\"
	
	CreateShortcut "$SMPROGRAMS\${Product_Name}\${Product_Name}.lnk" "$INSTDIR\nthLink.Wpf.exe"
	
	CreateShortcut "$DESKTOP\${Product_Name}.lnk" "$INSTDIR\nthLink.Wpf.exe"
	
	SectionEnd ; end the section
	
	Section "Uninstall"	
	; Make sure you are on the right path
    ${If} ${TargetPlatform} == "x64"
        SetRegView 64
    ${Else}
        SetRegView 32
    ${EndIf}
	
	DeleteRegKey HKLM "SOFTWARE\${Product_Name}"
	DeleteRegKey HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${Product_Name}"
	
	SetShellVarContext all
	
	Delete "$SMPROGRAMS\${Product_Name}\${Product_Name}.lnk"	
	Delete "$DESKTOP\${Product_Name}.lnk"
	
	Delete "$INSTDIR\*.*"
	RMDir /r "$INSTDIR"
	
	SectionEnd