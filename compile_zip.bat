set PROJECT_NAME=AutoExecuteNode
set CONFIG=Debug

set OUTPUT=output
set LOCAL_DIR=%OUTPUT%\%PROJECT_NAME%

set ZIP_File=%PROJECT_NAME%.zip

echo %ZIP_File%

echo off

@REM create local dir
if not exist %OUTPUT% mkdir %OUTPUT%

rd /s/q %LOCAL_DIR%

if not exist %LOCAL_DIR% mkdir %LOCAL_DIR%

copy /Y LICENSE.md %LOCAL_DIR%\
copy /Y swinfo.json %LOCAL_DIR%\
copy /Y README.md %LOCAL_DIR%\

md %LOCAL_DIR%\assets
md %LOCAL_DIR%\assets\images
copy /Y icon.png %LOCAL_DIR%\assets\images

copy /Y %PROJECT_NAME%\obj\%CONFIG%\%PROJECT_NAME%.dll %LOCAL_DIR%

set CWD=%cd%

del %ZIP_File%

cd %OUTPUT%

"C:\Program Files\7-Zip\7z.exe" a %ZIP_File% %PROJECT_NAME%

cd %CWD%

echo Copy to target Ksp dir
echo on

set DEST_PATH="D:\SteamLibrary\steamapps\common\Kerbal Space Program 2\BepInEx\plugins\%PROJECT_NAME%"
echo dest path is : %DEST_PATH%

rd /s/q %DEST_PATH%
mkdir  %DEST_PATH%
if not exist %DEST_PATH% mkdir %DEST_PATH%
xcopy /s /d %LOCAL_DIR% %DEST_PATH%


@REM @REM create bin dir
@REM if not exist %DEST_PATH%/bin mkdir %DEST_PATH%/bin

@REM copy /Y LICENSE.md %DEST_PATH%\
@REM copy /Y modInfo.json %DEST_PATH%\
@REM copy /Y README.md %DEST_PATH%\

@REM copy /Y icon.png %DEST_PATH%\bin
@REM copy /Y AutoExecuteNode/obj\%CONFIG%\%PROJECT_NAME%.dll %DEST_PATH%\bin

@REM echo end
@REM @REM pause


