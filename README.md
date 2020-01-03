When try to build:

```
$ dotnet build
Microsoft (R) Build Engine version 16.4.0+e901037fe for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

  Restore completed in 26.19 ms for /home/adam/projects/fsharp_playground/fsharp_playground.fsproj.
/home/adam/projects/fsharp_playground/UnitTest1.fs(26,9): error FS0071: Type constraint mismatch when applying the default type 'obj' for a type inference variable. No overloads match for method 'LiftAsync'. The available overloads are shown below. Consider adding further type constraints [/home/adam/projects/fsharp_playground/fsharp_playground.fsproj]


Build FAILED.
```

Verbose error:

```        
   1:7>/home/adam/projects/fsharp_playground/UnitTest1.fs(26,9): error FS0071: Type constraint mismatch when applying the default type 'obj' for a type inference variable. No overloads match for method 'LiftAsync'. The available overloads are shown below. Consider adding further type constraints [/home/adam/projects/fsharp_playground/fsharp_playground.fsproj]
         Possible overload: 'static member Control.LiftAsync.LiftAsync :  ^R -> (Async<'T> ->  ^R) when  ^R : (static member LiftAsync : Async<'T> ->  ^R)'. Type constraint mismatch. The type     'obj'    is not compatible with type    ''a'    .
         Possible overload: 'static member Control.LiftAsync.LiftAsync :  ^t -> unit when  ^t : null and  ^t : struct'. Type constraint mismatch. The type     'obj'    is not compatible with type    ''a'    .
         Possible overload: 'static member Control.LiftAsync.LiftAsync : Async<'T> -> (Async<'T> -> Async<'T>)'. Type constraint mismatch. The type     'obj'    is not compatible with type    'Async<'a>'    .
         The command exited with code 1.
```

If the subsequent usage of `response` is taken out, it compiles ok. Why?

Also, if I annotate the type of response like this:
```fsharp
let! (response : HttpResponse) =
    utcNow |> recordTimestampUsingFSData |> liftAsync
```
it works!

_However,_ In my project I am having this issue on, it doesn't. I can't tell the difference.

My work around I have found is I have to do this:

```fsharp
let! response =
    updateExchangeProfile url updatedProfile
    |> (liftAsync : Async<HttpResponse> -> ResultT<Async<Result<HttpResponse,ExchangeError>>>)
```

or use specific functions:

```fsharp
let! response =
    updateExchangeProfile url updatedProfile
    |> Async.map Ok |> ResultT    
```
