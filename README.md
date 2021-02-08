# Defunc
Defunc is an esoteric programming language made by me. This github repository is to access the interpreter I made. Contact me on Discord (Obvious#5283) or join the small server I made for defunc specifically: https://discord.gg/TqNNbDFhwe
## Documentation
This minimalist function-based esolang has only 5 pre-defined functions, however you can also make your own functions.
### Pre-defined functions
Lowercase letters should in practice be substituted by other functions.
function|syntax|execution
-|-|-
`0`|`0`|Returns 0.
`+`|`+a`|Returns a+1.
`?`|`?abcd`|If a>b returns c, otherwise returns d.
`.`|`.a`|Returns a, with the side effect of writing it to the log.
`,`|`,`|Returns the response to a requested user input.

Note that `?` will execute both a and b, but only one of c and d.<br>
Note that any `,` will automatically be executed at a user-defined function, even if it wasn't necessary for the functions thereafter.
### User-defined functions
User-defined functions work a bit different to pre-defined functions. Instead of executing a unique operation, they are used to combine several of them. These user-defined functions can refer to functions that have been defined earlier, but also to themselves, creating a looping mechanism. The syntax is as follows:
```
[function name][variable names][definition]
```
Function names can however only be a single character long, and a character can't be used for this if it has been used before in the program as either a function or a variable. Variable names can also be only one character long, and these characters can not already be used by any functions. Any amount of variables can be put into a function, but every function can only return exactly one value.
### Other rules
There are three other rules rules to keep in mind while writing a defunc program:
* A line of code is either a definition (a function being defined) or an execution (a set of functions being executed).
* Variables are local and do not carry over from one function to another.
* An execution will not actually write its value unless told to.
## Examples
### Cat program
```
C?0.,0C
C
```
### Truth machine
```
Ta?.a0Ta0
T,
```
### Addition
```
Sabc?bcS+ab+ca
S,,0
```
### Multiplication
```
Sabc?bcS+ab+ca
Pabcd?bcPab+cSad0d
P,,00
```
### Exponentiation
```
Sabc?bcS+ab+ca
Pabcd?bcPab+cSad0d
Eabcd?bcEab+cPad00d
E,,00
```
### Fibonacci Sequence
```
Sabc?bcS+ab+ca
Fab?0.a0FbSab0
F0+0
```
### Prime Factorisation
```
Sabc?bcS+ab+ca
Qabcd?Sbc0adQabSbc0+d
Dabc?acDabSbc0?ca0+0
Fab?a+0?Dab00?0.b0FQab00bFa+b0
F,++0
```
