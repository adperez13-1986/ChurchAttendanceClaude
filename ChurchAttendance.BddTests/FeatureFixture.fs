module ChurchAttendance.BddTests.FeatureFixture

open TickSpec.Xunit
open global.Xunit

let source = AssemblyStepDefinitionsSource(System.Reflection.Assembly.GetExecutingAssembly())
let scenarios resourceName = source.ScenariosFromEmbeddedResource resourceName |> MemberData.ofScenarios

[<Theory; MemberData("scenarios", "ChurchAttendance.BddTests.Features.MemberManagement.feature")>]
let ``Member Management`` (scenario: XunitSerializableScenario) = source.RunScenario(scenario)

[<Theory; MemberData("scenarios", "ChurchAttendance.BddTests.Features.AttendanceTracking.feature")>]
let ``Attendance Tracking`` (scenario: XunitSerializableScenario) = source.RunScenario(scenario)

[<Theory; MemberData("scenarios", "ChurchAttendance.BddTests.Features.Reporting.feature")>]
let ``Reporting`` (scenario: XunitSerializableScenario) = source.RunScenario(scenario)

