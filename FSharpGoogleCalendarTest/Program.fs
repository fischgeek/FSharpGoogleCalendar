namespace FSharpGoogleCalendarTest

open FSharpGoogleCalendar
open System

module Main =
    [<EntryPoint>]
    let main argv =      
        let calendarId = "8vr7je3qbbcaep27ccjf4kbjng@group.calendar.google.com"
        let startDate = DateTime.Now
        let endDate = DateTime.Now.AddDays(2.)
        
        let firstGroup() = 
            GoogleCalendar.GetEventList calendarId startDate endDate
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

        let events = GoogleCalendar.GetEventList "fischious@gmail.com" startDate endDate

        events
        |> function
        | Some list -> 
            list 
            |> Seq.iter (fun event -> 
                printfn "--------------------------------------------------"
                printfn "%s [%A]" event.Summary event.Start.DateTimeRaw
                printfn "%s" event.Location
            )
        | None -> 
            printfn "No events."
            ()
        //let bg,fg = GoogleCalendar.GetCalendarColor "primary"
        GoogleCalendar.GetAllCalendars()
        Console.ReadLine() |> ignore
        0
