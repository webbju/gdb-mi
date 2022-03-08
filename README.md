# gdb-mi

![example workflow](https://github.com/webbju/gdb-mi/actions/workflows/main.yaml/badge.svg) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.md)

### Synopsis

An easy-to-use management library for projects which use **[GDB/MI](https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI.html#GDB_002fMI)** or **[lldb-mi](https://github.com/lldb-tools/lldb-mi)** to coordinate asynchronously with GDB/LLDB debugging sessions. Provides abstract types and utilities to aide working with the Machine Interface's **[output syntax](https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Output-Syntax.html#GDB_002fMI-Output-Syntax)**. This project is an evolution of the approach used in **[Android++](https://github.com/webbju/android-plus-plus)**.

### What is GDB/MI?

> _GDB/MI is a line based machine oriented text interface to GDB and is activated by specifying using the --interpreter command line option (see Mode Options). It is specifically intended to support the development of systems which use the debugger as just one small component of a larger system._

https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI.html#GDB_002fMI
