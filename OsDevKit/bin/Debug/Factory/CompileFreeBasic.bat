setlocal
cd Factory
cd ..
cd freebasic
fbc.exe -c %1 -o ..\Factory\Build\%2.o 
cd ..
cd Factory