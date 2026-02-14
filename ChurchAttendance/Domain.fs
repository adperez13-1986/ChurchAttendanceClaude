namespace ChurchAttendance

open System
open System.Text.Json.Serialization

[<JsonFSharpConverter>]
type AgeGroup =
    | Men
    | Women
    | YAN
    | CYN
    | Children
    | Infants

[<JsonFSharpConverter>]
type Category =
    | Member
    | Attendee
    | Visitor
    | UnderMonitoring

[<JsonFSharpConverter>]
type ServiceType =
    | SundayService
    | PrayerMeeting

type Member =
    { Id: Guid
      FullName: string
      AgeGroup: AgeGroup
      Category: Category
      DateRegistered: DateTime
      FirstAttendedDate: DateTime option
      IsActive: bool }

type AttendanceRecord =
    { Id: Guid
      Date: DateTime
      ServiceType: ServiceType
      MemberIds: Guid list }

module Domain =

    let ageGroupLabel =
        function
        | Men -> "Men"
        | Women -> "Women"
        | YAN -> "YAN"
        | CYN -> "CYN"
        | Children -> "Children"
        | Infants -> "Infants"

    let parseAgeGroup =
        function
        | "Men" -> Some Men
        | "Women" -> Some Women
        | "YAN" -> Some YAN
        | "CYN" -> Some CYN
        | "Children" -> Some Children
        | "Infants" -> Some Infants
        | _ -> None

    let categoryLabel =
        function
        | Member -> "Member"
        | Attendee -> "Attendee"
        | Visitor -> "Visitor"
        | UnderMonitoring -> "Under Monitoring"

    let parseCategory =
        function
        | "Member" -> Some Member
        | "Attendee" -> Some Attendee
        | "Visitor" -> Some Visitor
        | "UnderMonitoring" -> Some UnderMonitoring
        | _ -> None

    let serviceTypeLabel =
        function
        | SundayService -> "Sunday Service"
        | PrayerMeeting -> "Prayer Meeting"

    let parseServiceType =
        function
        | "SundayService" -> Some SundayService
        | "PrayerMeeting" -> Some PrayerMeeting
        | _ -> None

    let allServiceTypes = [ SundayService; PrayerMeeting ]

    let allAgeGroups = [ Men; Women; YAN; CYN; Children; Infants ]
    let allCategories = [ Member; Attendee; UnderMonitoring; Visitor ]

    let newMember name ageGroup category firstAttendedDate =
        { Id = Guid.NewGuid()
          FullName = name
          AgeGroup = ageGroup
          Category = category
          DateRegistered = DateTime.Today
          FirstAttendedDate = firstAttendedDate
          IsActive = true }

    let isFirstTimer (m: Member) (date: DateTime) =
        match m.FirstAttendedDate with
        | Some firstDate -> firstDate.Date = date.Date
        | None -> false

