set PROJECT_NAME=AutoExecuteNode
set CONFIG=Debug

set OUTPUT=output
set LOCAL_DIR=%OUTPUT%\%PROJECT_NAME%

set ZIP_File=%PROJECT_NAME%.zip

echo %ZIP_File%

echo off

echo ####################### make zip #######################

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

dir