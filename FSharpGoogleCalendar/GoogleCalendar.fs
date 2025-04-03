namespace FSharpGoogleCalendar

open Google.Apis.Auth.OAuth2
open Google.Apis.Calendar.v3
open Google.Apis.Services
open Google.Apis.Util.Store
open System
open System.IO
open System.Threading

module GoogleCalendar =

    let private scopes = [| CalendarService.Scope.CalendarReadonly; CalendarService.Scope.CalendarEvents |]

    // https://developers.google.com/calendar/quickstart/dotnet
    let private credsFile = @"c:\dev\config\google-calendar-credentials.json"
    let private credPath = "token.json"

    let private Authenticate() =
        let stream = new FileStream(credsFile, FileMode.Open, FileAccess.Read)
        let credential =
            (GoogleWebAuthorizationBroker.AuthorizeAsync
                (GoogleClientSecrets.FromStream(stream).Secrets, scopes, "user", CancellationToken.None, new FileDataStore(credPath, true))).Result
        credential

    let private CreateGoogleCalendarService credential =
        let baseClientService = new BaseClientService.Initializer()
        baseClientService.HttpClientInitializer <- credential
        baseClientService.ApplicationName <- "Application Name"
        new CalendarService(baseClientService)

    let private CreateRequest calId minDate maxDate service =
        let request = new EventsResource.ListRequest(service, calId)
        request.TimeMin <- Nullable minDate
        request.TimeMax <- Nullable maxDate
        request.ShowDeleted <- Nullable false
        request.SingleEvents <- Nullable true
        //request.MaxResults <- Nullable 10
        request.OrderBy <- Nullable EventsResource.ListRequest.OrderByEnum.StartTime
        request

    let private createUpdateRequest calId event colorId service = 
        let request = new EventsResource.UpdateRequest(service, event, calId, event.Id)
        request

    let GetAllCalendars () =
        printfn "All Calendars"
        let service = Authenticate() |> CreateGoogleCalendarService
        let myCals = service.CalendarList.List().Execute()
        myCals.Items
        |> Seq.toList
        |> Seq.iter (fun cal -> 
            printfn $"-{cal.Summary}"
            printfn $"\tId: {cal.Id}"
            printfn $"\tSummary: {cal.Summary}"
            printfn $"\tSummaryO: {cal.SummaryOverride}"
            printfn $"\tDescription: {cal.Description}"
            printfn $"\tAccess Role: {cal.AccessRole}"
        )

    let private GetEvents(request: EventsResource.ListRequest) =
        try
            let events = request.Execute()
            events.Items
            |> Seq.toList
            |> function
            | [] -> None
            | list -> Some list
        with ex -> 
            printfn $"Calendar not found: {request.CalendarId}"
            None

    let GetEventList calId startDate endDate =
        Authenticate()
        |> CreateGoogleCalendarService
        |> CreateRequest calId startDate endDate
        |> GetEvents

    let UpdateEventColor calId eventId colorId = 
        let req = 
            Authenticate()
            |> CreateGoogleCalendarService
            |> createUpdateRequest calId eventId colorId
        try
            req.Execute()
        with ex -> failwith ex.Message

    let GetCalendarColor calId = 
        let service = 
            Authenticate() 
            |> CreateGoogleCalendarService
        try
            let myCal = service.CalendarList.Get(calId).Execute()
            Some (myCal.BackgroundColor,myCal.ForegroundColor)
        with ex -> None
        

    let GetColors colorId =
        let service = Authenticate() |> CreateGoogleCalendarService
        let clrs = service.Colors.Get().Execute()
        let dictionary = clrs.Calendar
        let c = dictionary.Item(colorId)
        //printfn $"ETag: {c.ETag}"
        //printfn $"FG: {c.Foreground}"
        //printfn $"BG: {c.Background}"
        c