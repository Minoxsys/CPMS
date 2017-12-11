using System;
using System.Configuration;
using CPMS.Authorization;
using CPMS.Configuration;
using CPMS.Patient.Domain;

namespace DBInitializer.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<UnitOfWork>
    {
        private readonly bool _seedForTesting;

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "DBInitializer.UnitOfWork";
            _seedForTesting = bool.Parse(ConfigurationManager.AppSettings["SeedForTesting"]);
        }

        protected override void Seed(UnitOfWork context)
        {
            if (_seedForTesting)
            {
                SeedPatients(context);
                SeedHospitalSpecialties(context);
            }
            SeedEvents(context);
            SeedUsersRolesAndPermissions(context);
        }

        private void SeedPatients(UnitOfWork context)
        {
            context.Patients.AddOrUpdate(patient => patient.NHSNumber,
                new Patient { Title = "Mr", Name = "Earl Grey", DateOfBirth = new DateTime(1980, 2, 4), NHSNumber = "1234567890", ConsultantName = "Consultant 1", ConsultantNumber = "ConsultantNr1" },
                new Patient { Title = "Mr", Name = "Dan Miller", DateOfBirth = new DateTime(1949, 12, 5), NHSNumber = "1244567890", ConsultantName = "Consultant 2", ConsultantNumber = "ConsultantNr2" },
                new Patient { Title = "Mrs", Name = "Helen Bryan", DateOfBirth = new DateTime(1965, 3, 8), NHSNumber = "1254567890", ConsultantName = "Consultant 3", ConsultantNumber = "ConsultantNr3" },
                new Patient { Title = "Ms", Name = "Abigail Howard", DateOfBirth = new DateTime(1986, 7, 13), NHSNumber = "1264567890", ConsultantName = "Consultant 4", ConsultantNumber = "ConsultantNr4" },
                new Patient { Title = "Mrs", Name = "Joan Smithfield", DateOfBirth = new DateTime(1974, 1, 5), NHSNumber = "1274567890", ConsultantName = "Consultant 5", ConsultantNumber = "ConsultantNr5" },
                new Patient { Title = "Mrs", Name = "Elizabeth Gondek", DateOfBirth = new DateTime(1958, 8, 4), NHSNumber = "1284567890", ConsultantName = "Consultant 6", ConsultantNumber = "ConsultantNr6" },
                new Patient { Title = "Mrs", Name = "Annie Radebaugh", DateOfBirth = new DateTime(1961, 12, 4), NHSNumber = "1294567890", ConsultantName = "Consultant 7", ConsultantNumber = "ConsultantNr7" },
                new Patient { Title = "Ms", Name = "Naomi Stoltenberg", DateOfBirth = new DateTime(1977, 1, 7), NHSNumber = "1304567890", ConsultantName = "Consultant 8", ConsultantNumber = "ConsultantNr8" },
                new Patient { Title = "Mr", Name = "Forrest Loveridge", DateOfBirth = new DateTime(1984, 2, 8), NHSNumber = "1314567890", ConsultantName = "Consultant 9", ConsultantNumber = "ConsultantNr9" },
                new Patient { Title = "Mr", Name = "Octavio Talmage", DateOfBirth = new DateTime(1990, 6, 10), NHSNumber = "1324567890", ConsultantName = "Consultant 10", ConsultantNumber = "ConsultantNr10" },
                new Patient { Title = "Mrs", Name = "Diana Fontanez", DateOfBirth = new DateTime(1938, 9, 9), NHSNumber = "1344567890", ConsultantName = "Consultant 11", ConsultantNumber = "ConsultantNr11" },
                new Patient { Title = "Mr", Name = "Neil Meyerson", DateOfBirth = new DateTime(1959, 11, 1), NHSNumber = "1354567890", ConsultantName = "Consultant 12", ConsultantNumber = "ConsultantNr12" },
                new Patient { Title = "Ms", Name = "Renata Franke", DateOfBirth = new DateTime(1965, 7, 18), NHSNumber = "1364567890", ConsultantName = "Consultant 13", ConsultantNumber = "ConsultantNr13" },
                new Patient { Title = "Ms", Name = "Ann Frankness", DateOfBirth = new DateTime(1957, 4, 27), NHSNumber = "1374567890", ConsultantName = "Consultant 14", ConsultantNumber = "ConsultantNr14" },
                new Patient { Title = "Mrs", Name = "Chloe Sun", DateOfBirth = new DateTime(1964, 8, 15), NHSNumber = "1384567890", ConsultantName = "Consultant 15", ConsultantNumber = "ConsultantNr15" });
        }


        private void SeedHospitalSpecialties(UnitOfWork context)
        {
            var hospital1 = new Hospital { Name = "Hospital 1" };
            var hospital2 = new Hospital { Name = "Hospital 2" };
            var hospital3 = new Hospital { Name = "Hospital 3" };

            var specialty1 = new Specialty { Code = "310", Name = "Audiology", Hospitals = new[] { hospital2 } };
            var specialty2 = new Specialty { Code = "330", Name = "Dermatology", Hospitals = new[] { hospital2 } };
            var specialty3 = new Specialty { Code = "302", Name = "Endocrinology", Hospitals = new[] { hospital2 } };
            var specialty4 = new Specialty { Code = "120", Name = "ENT", Hospitals = new[] { hospital1 } };
            var specialty5 = new Specialty { Code = "100", Name = "General Surgery", Hospitals = new[] { hospital1 } };
            var specialty6 = new Specialty { Code = "360", Name = "Genito-Urinary", Hospitals = new[] { hospital2 } };
            var specialty7 = new Specialty { Code = "101", Name = "Urology", Hospitals = new[] { hospital2 } };
            var specialty8 = new Specialty { Code = "400", Name = "Neurology", Hospitals = new[] { hospital1, hospital2 } };
            var specialty9 = new Specialty { Code = "800", Name = "Oncology", Hospitals = new[] { hospital3 } };
            var specialty10 = new Specialty { Code = "130", Name = "Ophthalmology", Hospitals = new[] { hospital2 } };
            var specialty11 = new Specialty { Code = "143", Name = "Orthopaedics", Hospitals = new[] { hospital2 } };
            var specialty12 = new Specialty { Code = "160", Name = "Plastic Surgery", Hospitals = new[] { hospital1 } };
            var specialty13 = new Specialty { Code = "410", Name = "Rheumatology", Hospitals = new[] { hospital2 } };
            

            hospital1.Specialties = new[] { specialty4, specialty8, specialty12, specialty5};
            hospital2.Specialties = new[] { specialty13, specialty11, specialty1, specialty2, specialty3, specialty6, specialty10, specialty7, specialty8};
            hospital3.Specialties = new[] { specialty9};
            
            context.Specialties.AddOrUpdate(specialty => specialty.Code, specialty1, specialty2, specialty3, specialty4, specialty5, specialty6, specialty7, specialty8, specialty9, specialty10, specialty11, specialty12, specialty13);

            context.SaveChanges();
        }

        private void SeedEvents(UnitOfWork context)
        {
            context.SourceEvents.AddOrUpdate(ev => ev.SourceCode,
            new SourceEvent
            {
                SourceCode = ConfigurationEventCode.ReferralReceived,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationCode = ConfigurationEventCode.FurtherInformationRequired, IsMandatory = false, TargetNumberOfDays = 14, EventForDateReferenceForTarget = ConfigurationEventCode.ReferralReceived , ClockType = ClockType.None},
                    new DestinationEvent {DestinationCode = ConfigurationEventCode.AddToTriageWaitingList, IsMandatory = true, TargetNumberOfDays = 21, EventForDateReferenceForTarget = ConfigurationEventCode.ReferralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceCode = ConfigurationEventCode.FurtherInformationRequired
            },
            new SourceEvent
            {
                SourceCode = ConfigurationEventCode.AddToTriageWaitingList,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationCode = ConfigurationEventCode.TriageReferral, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = ConfigurationEventCode.AddToTriageWaitingList, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceCode = ConfigurationEventCode.TriageReferral,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationCode = ConfigurationEventCode.AddPatientToAppointmentWaitingList, IsMandatory = true, TargetNumberOfDays = 2, EventForDateReferenceForTarget = ConfigurationEventCode.TriageReferral, ClockType = ClockType.None},
                    new DestinationEvent {DestinationCode = ConfigurationEventCode.NewlyDiagnosedStructuredEducation, IsMandatory = true, TargetNumberOfDays = 2, EventForDateReferenceForTarget = ConfigurationEventCode.TriageReferral, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceCode = ConfigurationEventCode.AddPatientToAppointmentWaitingList,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationCode = ConfigurationEventCode.BookAppt, IsMandatory = true, TargetNumberOfDays = 21, EventForDateReferenceForTarget = ConfigurationEventCode.AddPatientToAppointmentWaitingList, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceCode = ConfigurationEventCode.NewlyDiagnosedStructuredEducation
            },
            new SourceEvent
            {
                SourceCode = ConfigurationEventCode.BookAppt,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationCode = ConfigurationEventCode.AppointmentDate, IsMandatory = true, TargetNumberOfDays = 60, EventForDateReferenceForTarget = ConfigurationEventCode.BookAppt, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceCode = ConfigurationEventCode.AppointmentDate,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationCode = ConfigurationEventCode.OutcomeAppointment, IsMandatory = true, TargetNumberOfDays = 1, EventForDateReferenceForTarget = ConfigurationEventCode.AppointmentDate, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceCode = ConfigurationEventCode.PatientAttends
            },
            new SourceEvent
            {
                SourceCode = ConfigurationEventCode.OutcomeAppointment
            });
        }

        private void SeedUsersRolesAndPermissions(UnitOfWork context)
        {
            var permission1 = new Permission { Id = PermissionId.ManageUsers, Name = "Manage Users" };
            var permission2 = new Permission { Id = PermissionId.EditRolesActivitiesMapping, Name = "Edit Roles-Permissions Mapping" };
            var permission3 = new Permission { Id = PermissionId.ManagePlannedEvents, Name = "Manage Planned Events" };
            var permission4 = new Permission { Id = PermissionId.Patient, Name = "Patient Information" };
            var permission5 = new Permission { Id = PermissionId.EventBreaches, Name = "Event Breaches Information" };
            var permission6 = new Permission { Id = PermissionId.RTTPeriodBreaches, Name = "RTT Period Breaches Information" };
            var permission7 = new Permission { Id = PermissionId.Reports, Name = "Reports Information" };
            var permission8 = new Permission { Id = PermissionId.ActionsNotifications, Name = "Actions Notifications" };
            var permission9 = new Permission { Id = PermissionId.BreachesNotifications, Name = "Breaches Notifications" };
            var permission10 = new Permission { Id = PermissionId.ErrorsNotifications, Name = "Errors Notifications" };
            var permission11 = new Permission { Id = PermissionId.ImportsNotifications, Name = "Imports Notifications" };
            var permission12 = new Permission { Id = PermissionId.Trust, Name = "Trust Information" };


            var role1 = new Role { Name = "Admin", Permissions = new[] { permission1, permission2, permission3, permission4, permission5, permission6, permission7, permission8, permission9, permission10, permission11, permission12 } };
            var role2 = new Role { Name = "Clinician", Permissions = new[] { permission1, permission2, permission3, permission4, permission8, permission9 } };
            var role3 = new Role { Name = "Clerk", Permissions = new[] { permission6 } };
            var role4 = new Role { Name = "PAS Clerk", Permissions = new[] { permission9 } };
            var role5 = new Role { Name = "Waiting List Clerk", Permissions = new[] { permission7 } };
            var role6 = new Role { Name = "Trust Admin", Permissions = new[] { permission7 } };
            var role7 = new Role { Name = "Hospital Manager", Permissions = new[] { permission1, permission2, permission3, permission4, permission8, permission9 } };
            var role8 = new Role { Name = "Support", Permissions = new[] { permission9 } };


            permission1.Roles = new[] { role1, role2, role7 };
            permission2.Roles = new[] { role1, role2, role7 };
            permission3.Roles = new[] { role1, role2, role7 };
            permission4.Roles = new[] { role1, role2, role7 };
            permission5.Roles = new[] { role1 };
            permission6.Roles = new[] { role1, role3 };
            permission7.Roles = new[] { role1, role5, role6 };
            permission8.Roles = new[] { role1, role2, role7 };
            permission9.Roles = new[] { role1, role2, role4, role7, role8 };
            permission10.Roles = new[] {role1};
            permission11.Roles = new[] {role1};
            permission12.Roles = new[] { role1 };

            var user1 = new User
            {
                Username = "admin",
                Password = "admin",
                FullName = "Bill Smith",
                Email = "bill.smith@yahoo.com",
                IsActive = true,
                Role = role1
            };
            if (_seedForTesting)
            {
                var user2 = new User
                {
                    Username = "pasclerk",
                    Password = "pasclerk",
                    FullName = "John Doe",
                    Email = "john.doe@yahoo.com",
                    IsActive = true,
                    Role = role4
                };

                context.Users.AddOrUpdate(p => p.Username, user2);
            }
            context.Users.AddOrUpdate(p => p.Username, user1);
        }
    }
}
