namespace FSharpGoogleCalendarTest

open FSharpGoogleCalendar
open System

module Main =
    [<EntryPoint>]
    let main argv =
        GoogleCalendar.GetEventList()
        |> function
        | Some list -> 
            list 
            |> Seq.iter (fun event -> 
                printfn "--------------------------------------------------"
                printfn "%s [%A]" event.Summary event.Start.DateTime.Value
                printfn "%s" event.Location
            )
        | None -> 
            printfn "No events."
            ()
        Console.ReadLine() |> ignore
        0
