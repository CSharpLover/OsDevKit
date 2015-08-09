ENTRY (loader)
OUTPUT ("kernel.bin")

addr = 0x100000;

SECTIONS
{
   . = 0x00100000;

    .text :{
        KERNEL_START = .;

        *(.text)
    }

    .rodata ALIGN (0x1000) : {
        *(.rodata)
        _CTORS = .;
        *(.ctors)
        _CTORS_END = .;       
    }

    .data ALIGN (0x1000) : {
        *(.data)
    }

    .bss : {
        SBSS = .;
        *(COMMON)
        *(.bss)
        EBSS = .;

        KERNEL_END = .;
    }

}

