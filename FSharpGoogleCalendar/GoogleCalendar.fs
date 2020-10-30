namespace FSharpGoogleCalendar

open Google.Apis.Auth.OAuth2
open Google.Apis.Calendar.v3
open Google.Apis.Services
open Google.Apis.Util.Store
open System
open System.IO
open System.Threading

module GoogleCalendar =

    let private scopes = [| CalendarService.Scope.CalendarReadonly |]

    // https://developers.google.com/calendar/quickstart/dotnet
    let private credsFile = @"c:\dev\config\google-calendar-credentials.json"
    let private credPath = "token.json"

    let private Authenticate() =
        let stream = new FileStream(credsFile, FileMode.Open, FileAccess.Read)
        let credential =
            (GoogleWebAuthorizationBroker.AuthorizeAsync
                (GoogleClientSecrets.Load(stream).Secrets, scopes, "user", CancellationToken.None, new FileDataStore(credPath, true))).Result
        credential

    let private CreateGoogleCalendarService credential =
        let baseClientService = new BaseClientService.Initializer()
        baseClientService.HttpClientInitializer <- credential
        baseClientService.ApplicationName <- "Application Name"
        new CalendarService(baseClientService)

    let private CreateRequest minDate maxDate service =
        let request = new EventsResource.ListRequest(service, "primary")
        request.TimeMin <- Nullable minDate
        request.TimeMax <- Nullable maxDate
        request.ShowDeleted <- Nullable false
        request.SingleEvents <- Nullable true
        request.MaxResults <- Nullable 10
        request.OrderBy <- Nullable EventsResource.ListRequest.OrderByEnum.StartTime
        request

    let private GetEvents(request: EventsResource.ListRequest) =
        try
            let events = request.Execute()
            events.Items
            |> Seq.toList
            |> function
            | [] -> None
            | list -> Some list
        with ex -> failwith ex.Message

    let GetEventList() =
        let startDate = DateTime.Now
        let endDate = DateTime.Now
        Authenticate()
        |> CreateGoogleCalendarService
        |> CreateRequest startDate endDate
        |> GetEvents
    ()
