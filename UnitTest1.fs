module fsharp_playground

open NUnit.Framework
open FSharpPlus
open FSharpPlus.Data
open FSharp.Data
open System

// Simulate sync action, that returns count of records synced
let syncRecords upToTimestamp : ResultT<Async<Result<int,string>>> = ResultT <| async.Return (Ok 3)

let recordTimestampUsingFSData body : Async<FSharp.Data.HttpResponse> =
    FSharp.Data.Http.AsyncRequest
        ( "https://myapi.example.com/sync"
        , httpMethod = "POST"
        , body = TextRequest (sprintf "%A" body) // actually is json
        , silentHttpErrors = true )

let performSync1 () =
    let utcNow = DateTime.UtcNow
    monad {
        let! count = syncRecords utcNow
        // This fails to compile with
        //    error FS0071: Type constraint mismatch when applying the default type 'obj' 
        //    for a type inference variable. No overloads match for method 'LiftAsync'.
        let! response =
            utcNow |> recordTimestampUsingFSData |> liftAsync

        // but compiles OK, if the following two lines are commented
        if response.StatusCode < 200 || response.StatusCode >= 300 then
            printfn "Sync succeeded, but failed to update bookmark"

        return count
    }

