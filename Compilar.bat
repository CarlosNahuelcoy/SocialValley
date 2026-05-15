@echo off
title Social Valley - Build Script
color 0A
echo ============================================
echo     Social Valley - Auto Build Script
echo ============================================
echo Current time: %date% %time%
echo.

echo [1/2] Cleaning previous build...
dotnet clean
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Clean failed!
    goto :error
)
echo Clean completed successfully.

echo.
echo [2/2] Building mod...
dotnet build
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Build failed!
    goto :error
)

echo.
echo ============================================
echo        BUILD COMPLETED SUCCESSFULLY!
echo ============================================
echo The mod DLL has been generated and copied.
echo You can now test the mod in Stardew Valley.
echo ============================================
goto :end

:error
echo.
echo ============================================
echo           BUILD FAILED!
echo ============================================
echo Please check the error messages above.
echo ============================================

:end
echo.
echo Press any key to close...
pause >nul