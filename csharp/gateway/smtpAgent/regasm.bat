@echo off
setlocal

if "%PROCESSOR_ARCHITECTURE%"=="x86" (
	set fxpath=\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe
) ELSE (
	set fxpath=\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe
)
	
%fxpath% %~f1 /tlb:%~n1.tlb /codebase

endlocal
