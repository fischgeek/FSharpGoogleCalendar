namespace FSharpGoogleCalendarTest

open FSharpGoogleCalendar

module Main =
    [<EntryPoint>]
    let main argv =
        let calendarEvents = GoogleCalendar.GetEventList()
        0
