using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using CPMS.Authorization;
using CPMS.Configuration;
using CPMS.Domain;

namespace DBInitializer.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<UnitOfWork>
    {
        private readonly bool _seedForTesting;
        private const string NoLongerInUse = "no longer in use";

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
            SeedEventsConnection(context);
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
            var hospital4 = new Hospital { Name = "Hospital 4" };
            var hospital5 = new Hospital { Name = "Hospital 5" };

            var specialty1 = new Specialty { Code = "100", Name = "GENERAL SURGERY", Hospitals = new[] { hospital1 } };
            var specialty2 = new Specialty { Code = "101", Name = "UROLOGY", Hospitals = new[] { hospital1 } };
            var specialty3 = new Specialty { Code = "110", Name = "TRAUMA & ORTHOPAEDICS", Hospitals = new[] { hospital1 } };
            var specialty4 = new Specialty { Code = "120", Name = "ENT", Hospitals = new[] { hospital1 } };
            var specialty5 = new Specialty { Code = "130", Name = "OPHTHALMOLOGY", Hospitals = new[] { hospital1 } };
            var specialty6 = new Specialty { Code = "140", Name = "ORAL SURGERY", Hospitals = new[] { hospital1 } };
            var specialty7 = new Specialty { Code = "141", Name = "RESTORATIVE DENTISTRY", Hospitals = new[] { hospital1 } };
            var specialty8 = new Specialty { Code = "142", Name = "PAEDIATRIC DENTISTRY", Hospitals = new[] { hospital1 } };
            var specialty9 = new Specialty { Code = "143", Name = "ORTHODONTICS", Hospitals = new[] { hospital1 } };
            var specialty10 = new Specialty { Code = "145", Name = "ORAL & MAXILLO FACIAL SURGERY", Hospitals = new[] { hospital1 } };
            var specialty11 = new Specialty { Code = "146", Name = "ENDODONTICS", Hospitals = new[] { hospital1 } };
            var specialty12 = new Specialty { Code = "147", Name = "PERIODONTICS", Hospitals = new[] { hospital1 } };
            var specialty13 = new Specialty { Code = "148", Name = "PROSTHODONTICS", Hospitals = new[] { hospital1 } };
            var specialty14 = new Specialty { Code = "149", Name = "SURGICAL DENTISTRY", Hospitals = new[] { hospital1 } };
            var specialty15 = new Specialty { Code = "150", Name = "NEUROSURGERY", Hospitals = new[] { hospital1 } };
            var specialty16 = new Specialty { Code = "160", Name = "PLASTIC SURGERY", Hospitals = new[] { hospital1 } };
            var specialty17 = new Specialty { Code = "170", Name = "CARDIOTHORACIC SURGERY", Hospitals = new[] { hospital1 } };
            var specialty18 = new Specialty { Code = "171", Name = "PAEDIATRIC SURGERY", Hospitals = new[] { hospital1 } };
            var specialty19 = new Specialty { Code = "180", Name = "ACCIDENT & EMERGENCY", Hospitals = new[] { hospital1 } };
            var specialty20 = new Specialty { Code = "190", Name = "ANAESTHETICS", Hospitals = new[] { hospital1 } };
            var specialty21 = new Specialty { Code = "191", Name = NoLongerInUse, Hospitals = new[] { hospital1 } };
            var specialty22 = new Specialty { Code = "192", Name = "CRITICAL CARE MEDICINE", Hospitals = new[] { hospital1 } };
            var specialty23 = new Specialty { Code = "199", Name = "Non-UK provider; specialty function not known, treatment mainly surgical", Hospitals = new[] { hospital1 } };
            var specialty24 = new Specialty { Code = "300", Name = "GENERAL MEDICINE", Hospitals = new[] { hospital1, hospital2 } };
            var specialty25 = new Specialty { Code = "301", Name = "GASTROENTEROLOGY", Hospitals = new[] { hospital1, hospital2 } };
            var specialty26 = new Specialty { Code = "302", Name = "ENDOCRINOLOGY", Hospitals = new[] { hospital2 } };
            var specialty27 = new Specialty { Code = "303", Name = "CLINICAL HAEMATOLOGY", Hospitals = new[] { hospital2 } };
            var specialty28 = new Specialty { Code = "304", Name = "CLINICAL PHYSIOLOGY", Hospitals = new[] { hospital2 } };
            var specialty29 = new Specialty { Code = "305", Name = "CLINICAL PHARMACOLOGY", Hospitals = new[] { hospital2 } };
            var specialty30 = new Specialty { Code = "310", Name = "AUDIOLOGICAL MEDICINE", Hospitals = new[] { hospital2 } };
            var specialty31 = new Specialty { Code = "311", Name = "CLINICAL GENETICS", Hospitals = new[] { hospital2 } };
            var specialty32 = new Specialty { Code = "312", Name = "CLINICAL CYTOGENETICS and MOLECULAR GENETICS", Hospitals = new[] { hospital2 } };
            var specialty33 = new Specialty { Code = "313", Name = "CLINICAL IMMUNOLOGY and ALLERGY", Hospitals = new[] { hospital2 } };
            var specialty34 = new Specialty { Code = "314", Name = "REHABILITATION", Hospitals = new[] { hospital2 } };
            var specialty35 = new Specialty { Code = "315", Name = "PALLIATIVE MEDICINE", Hospitals = new[] { hospital2 } };
            var specialty36 = new Specialty { Code = "320", Name = "CARDIOLOGY", Hospitals = new[] { hospital2 } };
            var specialty37 = new Specialty { Code = "321", Name = "PAEDIATRIC CARDIOLOGY", Hospitals = new[] { hospital2 } };
            var specialty38 = new Specialty { Code = "325", Name = "SPORTS AND EXERCISE MEDICINE ", Hospitals = new[] { hospital2 } };
            var specialty39 = new Specialty { Code = "326", Name = "ACUTE INTERNAL MEDICINE", Hospitals = new[] { hospital2 } };
            var specialty40 = new Specialty { Code = "330", Name = "DERMATOLOGY", Hospitals = new[] { hospital2 } };
            var specialty41 = new Specialty { Code = "340", Name = "RESPIRATORY MEDICINE (also known as thoracic medicine)", Hospitals = new[] { hospital2 } };
            var specialty42 = new Specialty { Code = "350", Name = "INFECTIOUS DISEASES", Hospitals = new[] { hospital2 } };
            var specialty43 = new Specialty { Code = "352", Name = "TROPICAL MEDICINE", Hospitals = new[] { hospital2 } };
            var specialty44 = new Specialty { Code = "360", Name = "GENITOURINARY MEDICINE", Hospitals = new[] { hospital2 } };
            var specialty45 = new Specialty { Code = "361", Name = "NEPHROLOGY", Hospitals = new[] { hospital2 } };
            var specialty46 = new Specialty { Code = "370", Name = "MEDICAL ONCOLOGY", Hospitals = new[] { hospital2 } };
            var specialty47 = new Specialty { Code = "371", Name = "NUCLEAR MEDICINE", Hospitals = new[] { hospital2 } };
            var specialty48 = new Specialty { Code = "400", Name = "NEUROLOGY", Hospitals = new[] { hospital3 } };
            var specialty49 = new Specialty { Code = "401", Name = "CLINICAL NEURO-PHYSIOLOGY", Hospitals = new[] { hospital3 } };
            var specialty50 = new Specialty { Code = "410", Name = "RHEUMATOLOGY", Hospitals = new[] { hospital3 } };
            var specialty51 = new Specialty { Code = "420", Name = "PAEDIATRICS", Hospitals = new[] { hospital3 } };
            var specialty52 = new Specialty { Code = "421", Name = "PAEDIATRIC NEUROLOGY", Hospitals = new[] { hospital3 } };
            var specialty53 = new Specialty { Code = "430", Name = "GERIATRIC MEDICINE", Hospitals = new[] { hospital3 } };
            var specialty54 = new Specialty { Code = "450", Name = "DENTAL MEDICINE SPECIALTIES", Hospitals = new[] { hospital3 } };
            var specialty55 = new Specialty { Code = "451", Name = "SPECIAL CARE DENTISTRY", Hospitals = new[] { hospital3 } };
            var specialty56 = new Specialty { Code = "460", Name = "MEDICAL OPHTHALMOLOGY", Hospitals = new[] { hospital3 } };
            var specialty57 = new Specialty { Code = "499", Name = "Non-UK provider; specialty function not known, treatment mainly medical", Hospitals = new[] { hospital3 } };
            var specialty58 = new Specialty { Code = "500", Name = "OBSTETRICS and GYNAECOLOGY", Hospitals = new[] { hospital4 } };
            var specialty59 = new Specialty { Code = "501", Name = "OBSTETRICS", Hospitals = new[] { hospital4 } };
            var specialty60 = new Specialty { Code = "502", Name = "GYNAECOLOGY", Hospitals = new[] { hospital4 } };
            var specialty61 = new Specialty { Code = "504", Name = "COMMUNITY SEXUAL AND REPRODUCTIVE HEALTH", Hospitals = new[] { hospital4 } };
            var specialty62 = new Specialty { Code = "510", Name = NoLongerInUse, Hospitals = new[] { hospital4 } };
            var specialty63 = new Specialty { Code = "520", Name = NoLongerInUse, Hospitals = new[] { hospital4 } };
            var specialty64 = new Specialty { Code = "560", Name = "MIDWIFE EPISODE", Hospitals = new[] { hospital4 } };
            var specialty65 = new Specialty { Code = "600", Name = "GENERAL MEDICAL PRACTICE", Hospitals = new[] { hospital4 } };
            var specialty66 = new Specialty { Code = "601", Name = "GENERAL DENTAL PRACTICE", Hospitals = new[] { hospital4 } };
            var specialty67 = new Specialty { Code = "610", Name = NoLongerInUse, Hospitals = new[] { hospital4 } };
            var specialty68 = new Specialty { Code = "620", Name = NoLongerInUse, Hospitals = new[] { hospital4 } };
            var specialty69 = new Specialty { Code = "700", Name = "LEARNING DISABILITY", Hospitals = new[] { hospital4 } };
            var specialty70 = new Specialty { Code = "710", Name = "ADULT MENTAL ILLNESS", Hospitals = new[] { hospital4 } };
            var specialty71 = new Specialty { Code = "711", Name = "CHILD and ADOLESCENT PSYCHIATRY", Hospitals = new[] { hospital4 } };
            var specialty72 = new Specialty { Code = "712", Name = "FORENSIC PSYCHIATRY", Hospitals = new[] { hospital4 } };
            var specialty73 = new Specialty { Code = "713", Name = "PSYCHOTHERAPY", Hospitals = new[] { hospital4 } };
            var specialty74 = new Specialty { Code = "715", Name = "OLD AGE PSYCHIATRY", Hospitals = new[] { hospital4 } };
            var specialty75 = new Specialty { Code = "800", Name = "CLINICAL ONCOLOGY (previously RADIOTHERAPY)", Hospitals = new[] { hospital5 } };
            var specialty76 = new Specialty { Code = "810", Name = "RADIOLOGY", Hospitals = new[] { hospital5 } };
            var specialty77 = new Specialty { Code = "820", Name = "GENERAL PATHOLOGY", Hospitals = new[] { hospital5 } };
            var specialty78 = new Specialty { Code = "821", Name = "BLOOD TRANSFUSION", Hospitals = new[] { hospital5 } };
            var specialty79 = new Specialty { Code = "822", Name = "CHEMICAL PATHOLOGY", Hospitals = new[] { hospital5 } };
            var specialty80 = new Specialty { Code = "823", Name = "HAEMATOLOGY", Hospitals = new[] { hospital5 } };
            var specialty81 = new Specialty { Code = "824", Name = "HISTOPATHOLOGY", Hospitals = new[] { hospital5 } };
            var specialty82 = new Specialty { Code = "830", Name = "IMMUNOPATHOLOGY", Hospitals = new[] { hospital1, hospital5 } };
            var specialty83 = new Specialty { Code = "831", Name = "MEDICAL MICROBIOLOGY AND VIROLOGY", Hospitals = new[] { hospital5 } };
            var specialty84 = new Specialty { Code = "832", Name = NoLongerInUse, Hospitals = new[] { hospital5 } };
            var specialty85 = new Specialty { Code = "833", Name = "MEDICAL MICROBIOLOGY (also known as Microbiology and Bacteriology)", Hospitals = new[] { hospital5 } };
            var specialty86 = new Specialty { Code = "834", Name = "MEDICAL VIROLOGY", Hospitals = new[] { hospital5 } };
            var specialty87 = new Specialty { Code = "900", Name = "COMMUNITY MEDICINE", Hospitals = new[] { hospital5 } };
            var specialty88 = new Specialty { Code = "901", Name = "OCCUPATIONAL MEDICINE", Hospitals = new[] { hospital5 } };
            var specialty89 = new Specialty { Code = "902", Name = "COMMUNITY HEALTH SERVICES DENTAL", Hospitals = new[] { hospital5 } };
            var specialty90 = new Specialty { Code = "903", Name = "PUBLIC HEALTH MEDICINE", Hospitals = new[] { hospital5 } };
            var specialty91 = new Specialty { Code = "904", Name = "PUBLIC HEALTH DENTAL", Hospitals = new[] { hospital5 } };
            var specialty92 = new Specialty { Code = "950", Name = "NURSING EPISODE", Hospitals = new[] { hospital5 } };
            var specialty93 = new Specialty { Code = "960", Name = "ALLIED HEALTH PROFESSIONAL EPISODE", Hospitals = new[] { hospital5 } };
            var specialty94 = new Specialty { Code = "990", Name = NoLongerInUse, Hospitals = new[] { hospital5 } };

            hospital1.Specialties = new[] { specialty1, specialty2, specialty3, specialty4, specialty5, specialty6, specialty7, specialty8, specialty9, specialty10, specialty11, specialty12, specialty13, specialty14, specialty15, specialty16, specialty17, specialty18, specialty19, specialty20, specialty21, specialty22, specialty23, specialty24, specialty25, specialty82 };
            hospital2.Specialties = new[] { specialty24, specialty25, specialty26, specialty27, specialty28, specialty29, specialty30, specialty31, specialty32, specialty33, specialty34, specialty35, specialty36, specialty37, specialty38, specialty39, specialty40, specialty41, specialty42, specialty43, specialty44, specialty45, specialty46, specialty47 };
            hospital3.Specialties = new[] { specialty48, specialty49, specialty50, specialty51, specialty52, specialty53, specialty54, specialty55, specialty56, specialty57 };
            hospital4.Specialties = new[] { specialty58, specialty59, specialty60, specialty61, specialty62, specialty63, specialty64, specialty65, specialty66, specialty67, specialty68, specialty69, specialty70, specialty71, specialty72, specialty73, specialty74 };
            hospital5.Specialties = new[] { specialty75, specialty76, specialty77, specialty78, specialty79, specialty80, specialty81, specialty82, specialty83, specialty84, specialty85, specialty86, specialty87, specialty88, specialty89, specialty90, specialty91, specialty92, specialty93, specialty94 };

            context.Specialties.AddOrUpdate(specialty => specialty.Code, specialty1, specialty2, specialty3, specialty4, specialty5, specialty6, specialty7, specialty8, specialty9, specialty10, specialty11, specialty12, specialty13, specialty14, specialty15, specialty16, specialty17, specialty18, specialty19, specialty20, specialty21, specialty22, specialty23, specialty24, specialty25, specialty26, specialty27, specialty28, specialty29, specialty30, specialty31, specialty32, specialty33, specialty34, specialty35, specialty36, specialty37, specialty38, specialty39, specialty40, specialty41, specialty42, specialty43, specialty44, specialty45, specialty46, specialty47, specialty48, specialty49, specialty50, specialty51, specialty52, specialty53, specialty54, specialty55, specialty56, specialty57, specialty58, specialty59, specialty60, specialty61, specialty62, specialty63, specialty64, specialty65, specialty66, specialty67, specialty68, specialty69, specialty70, specialty71, specialty72, specialty73, specialty74, specialty75, specialty76, specialty77, specialty78, specialty79, specialty80, specialty81, specialty82, specialty83, specialty84, specialty85, specialty86, specialty87, specialty88, specialty89, specialty90, specialty91, specialty92, specialty93, specialty94);

            context.SaveChanges();
        }

        private void SeedEventsConnection(UnitOfWork context)
        {
            var referralReceived = new EventName { Code = EventCode.ReferralReceived, Description = "Referral Received" };
            var referralReview = new EventName { Code = EventCode.ReferralReview, Description = "Referral Review" };
            var bookedOutpatientFirstAppointment = new EventName { Code = EventCode.BookedOutpatientFirstAppointment, Description = "Booked Outpatient First Appointment" };
            var outpatientFirstAppointment = new EventName { Code = EventCode.OutpatientFirstAppointment, Description = "Outpatient First Appointment" };
            var attendedOutpatientFirstAppointment = new EventName { Code = EventCode.AttendedOutpatientFirstAppointment, Description = "Attended Outpatient First Appointment" };
            var outcomedOutpatientFirstAppointment = new EventName { Code = EventCode.OutcomedOutpatientFirstAppointment, Description = "Outcomed Outpatient First Appointment" };
            var diagnosticsOrderPlaced = new EventName { Code = EventCode.DiagnosticsOrderPlaced, Description = "Diagnostics Order Placed" };
            var diagnosticsTestResultAvailable = new EventName { Code = EventCode.DiagnosticsTestResultAvailable, Description = "Diagnostics Test Result Available" };
            var letterSent = new EventName { Code = EventCode.LetterSent, Description = "Letter Sent" };
            var outpatientDischarged = new EventName { Code = EventCode.OutpatientDischarged, Description = "Outpatient Discharged" };
            var bookedOutpatientFollowUpAppointment = new EventName { Code = EventCode.BookedOutpatientFollowUpAppointment, Description = "Booked Outpatient Follow Up Appointment" };
            var outpatientFollowUpAppointment = new EventName { Code = EventCode.OutpatientFollowUpAppointment, Description = "Outpatient Follow Up Appointment" };
            var attendedOutpatientFollowUpAppointment = new EventName { Code = EventCode.AttendedOutpatientFollowUpAppointment, Description = "Attended Outpatient Follow Up Appointment" };
            var outcomedOutpatientFollowUpAppointment = new EventName { Code = EventCode.OutcomedOutpatientFollowUpAppointment, Description = "Outcomed Outpatient Follow Up Appointment" };
            var patientAddedToInpatientWaitingList = new EventName { Code = EventCode.PatientAddedToInpatientWaitingList, Description = "Patient Added To Inpatient Waiting List" };
            var inpatientTCIOffer = new EventName { Code = EventCode.InpatientTCIOffer, Description = "Inpatient TCI Offer" };
            var inpatientTCIAgreed = new EventName { Code = EventCode.InpatientTCIAgreed, Description = "Inpatient TCI Agreed" };
            var inpatientTCI = new EventName { Code = EventCode.InpatientTCI, Description = "Inpatient TCI" };
            var bookedInpatientPreOperativeAssessment = new EventName { Code = EventCode.BookedInpatientPreOperativeAssessment, Description = "Booked Inpatient Pre Operative Assessment" };
            var inpatientPreOperativeAssessment = new EventName { Code = EventCode.InpatientPreOperativeAssessment, Description = "Inpatient Pre Operative Assessment" };
            var inpatientPatientAttendancePreOperativeAssessment = new EventName { Code = EventCode.InpatientPatientAttendancePreOperativeAssessment, Description = "Inpatient Patient Attendance Pre Operative Assessment" };
            var inpatientAdmitted = new EventName { Code = EventCode.InpatientAdmitted, Description = "Inpatient Admitted" };
            var expectedInpatientDischarged = new EventName { Code = EventCode.ExpectedInpatientDischarged, Description = "Expected Inpatient Discharged" };
            var inpatientDischarged = new EventName { Code = EventCode.InpatientDischarged, Description = "Inpatient Discharged" };
            var episodeClinicallyCoded = new EventName { Code = EventCode.EpisodeClinicallyCoded, Description = "Episode Clinically Coded" };
            var bookedPostInpatientFirstOutpatientAppointment = new EventName { Code = EventCode.BookedPostInpatientFirstOutpatientAppointment, Description = "Booked Post Inpatient First Outpatient Appointment" };
            var postInpatientFirstOutpatientFollowUpAppointment = new EventName { Code = EventCode.PostInpatientFirstOutpatientFollowUpAppointment, Description = "Post Inpatient First Outpatient Follow Up Appointment" };
            var attendedPostInpatientFirstOutpatientFollowUpAppointment = new EventName { Code = EventCode.AttendedPostInpatientFirstOutpatientFollowUpAppointment, Description = "Attended Post Inpatient First Outpatient Follow Up Appointment" };
            var outcomedPostInpatientFirstOutpatientFollowUpAppointment = new EventName { Code = EventCode.OutcomedPostInpatientFirstOutpatientFollowUpAppointment, Description = "Outcomed Post Inpatient First Outpatient Follow Up Appointment" };
            var bookedPostInpatientSecondOutpatientAppointment = new EventName { Code = EventCode.BookedPostInpatientSecondOutpatientAppointment, Description = "Booked Post Inpatient Second Outpatient Appointment" };
            var postInpatientSecondOutpatientFollowUpAppointment = new EventName { Code = EventCode.PostInpatientSecondOutpatientFollowUpAppointment, Description = "Post Inpatient Second Outpatient Follow Up Appointment" };
            var attendedPostInpatientSecondOutpatientFollowUpAppointment = new EventName { Code = EventCode.AttendedPostInpatientSecondOutpatientFollowUpAppointment, Description = "Attended Post Inpatient Second Outpatient Follow Up Appointment" };
            var outcomedPostInpatientSecondOutpatientFollowUpAppointment = new EventName { Code = EventCode.OutcomedPostInpatientSecondOutpatientFollowUpAppointment, Description = "Outcomed Post Inpatient Second Outpatient Follow Up Appointment" };
            var patientCancelEvent = new EventName { Code = EventCode.PatientCancelEvent, Description = "Patient Cancel" };
            var hospitalCancelEvent = new EventName { Code = EventCode.HospitalCancelEvent, Description = "Hospital Cancel" };
            var pathwayCompletion = new EventName { Code = EventCode.PathwayCompletion, Description = "Pathway Completion" };
            var didNotAttend = new EventName { Code = EventCode.DidNotAttend, Description = "Did Not Attend" };

            context.EventNames.AddOrUpdate(@event => @event.Code, referralReceived, referralReview,
                bookedOutpatientFirstAppointment, outpatientFirstAppointment, attendedOutpatientFirstAppointment,
                outcomedOutpatientFirstAppointment, diagnosticsOrderPlaced, diagnosticsTestResultAvailable, letterSent,
                outpatientDischarged, bookedOutpatientFollowUpAppointment, outpatientFollowUpAppointment,
                attendedOutpatientFollowUpAppointment, outcomedOutpatientFollowUpAppointment,
                patientAddedToInpatientWaitingList, inpatientTCIOffer, inpatientTCIAgreed, inpatientTCI,
                bookedInpatientPreOperativeAssessment,
                inpatientPreOperativeAssessment, inpatientPatientAttendancePreOperativeAssessment, inpatientAdmitted,
                expectedInpatientDischarged, inpatientDischarged, episodeClinicallyCoded,
                bookedPostInpatientFirstOutpatientAppointment,
                postInpatientFirstOutpatientFollowUpAppointment, attendedPostInpatientFirstOutpatientFollowUpAppointment,
                outcomedPostInpatientFirstOutpatientFollowUpAppointment, bookedPostInpatientSecondOutpatientAppointment,
                postInpatientSecondOutpatientFollowUpAppointment,
                attendedPostInpatientSecondOutpatientFollowUpAppointment,
                outcomedPostInpatientSecondOutpatientFollowUpAppointment, patientCancelEvent, hospitalCancelEvent,
                pathwayCompletion, didNotAttend);

            context.SaveChanges();

            var sourceEvents = context.SourceEvents.Include(s => s.NextPossibleEvents).ToList();
            var eventNames = context.EventNames.ToList();

            foreach (var eventName in eventNames)
            {
                var sourceEvent = sourceEvents.FirstOrDefault(@event => @event.SourceName.Code == eventName.Code);
                if (sourceEvent != null)
                {
                    if (sourceEvent.NextPossibleEvents != null)
                    {
                        foreach (var destinationEvent in sourceEvent.NextPossibleEvents.ToList())
                        {
                            context.DestinationEvents.Remove(destinationEvent);
                        }
                    }
                    sourceEvents.Remove(sourceEvent);
                    context.SourceEvents.Remove(sourceEvent);
                    context.EventNames.Remove(eventName);
                }
            }

            context.SaveChanges();

            context.SourceEvents.AddOrUpdate(@event => @event.Id,
            new SourceEvent
            {
                SourceName = referralReceived,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = referralReview, IsMandatory = false, TargetNumberOfDays = 5, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = referralReview,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = bookedOutpatientFirstAppointment, IsMandatory = true, TargetNumberOfDays = 10, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = bookedOutpatientFirstAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outpatientFirstAppointment, IsMandatory = true, TargetNumberOfDays = 10, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = outpatientFirstAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = attendedOutpatientFirstAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = outpatientFirstAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = patientCancelEvent, IsMandatory = false, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = hospitalCancelEvent, IsMandatory = false, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = didNotAttend, IsMandatory = false, ClockType = ClockType.ClockStopping}
                }
            },
            new SourceEvent
            {
                SourceName = attendedOutpatientFirstAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outcomedOutpatientFirstAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = attendedOutpatientFirstAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = outcomedOutpatientFirstAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outpatientDischarged, IsMandatory = true, ClockType = ClockType.ClockStopping},
                    new DestinationEvent {DestinationName = bookedOutpatientFollowUpAppointment, IsMandatory = false, TargetNumberOfDays = 5, EventForDateReferenceForTarget = outcomedOutpatientFirstAppointment, ClockType = ClockType.None}, 
                    new DestinationEvent {DestinationName = patientAddedToInpatientWaitingList, IsMandatory = true, TargetNumberOfDays = 10, EventForDateReferenceForTarget = outcomedOutpatientFirstAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = pathwayCompletion, IsMandatory = true,ClockType = ClockType.ClockStopping},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived}
                }
            },
            new SourceEvent
            {
                SourceName = diagnosticsOrderPlaced,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = diagnosticsTestResultAvailable, IsMandatory = true, TargetNumberOfDays = 7, EventForDateReferenceForTarget = diagnosticsOrderPlaced, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = diagnosticsTestResultAvailable,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = letterSent, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = attendedOutpatientFirstAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = letterSent,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outpatientDischarged, IsMandatory = true, ClockType = ClockType.ClockStopping},
                    new DestinationEvent {DestinationName = letterSent, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = attendedOutpatientFirstAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = patientAddedToInpatientWaitingList, IsMandatory = true, TargetNumberOfDays = 10, EventForDateReferenceForTarget = outcomedOutpatientFirstAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = outpatientDischarged,
                NextPossibleEvents = new[]
                {
                     new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = patientAddedToInpatientWaitingList,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = inpatientTCIOffer, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = patientAddedToInpatientWaitingList, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = inpatientTCIOffer,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = inpatientTCIAgreed, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = inpatientTCIOffer,ClockType = ClockType.ClockPausing}, 
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = inpatientTCIAgreed,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = inpatientTCI, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = patientAddedToInpatientWaitingList, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = patientCancelEvent, IsMandatory = false, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = hospitalCancelEvent, IsMandatory = false, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = didNotAttend, IsMandatory = false, ClockType = ClockType.ClockStopping}
                }
            },
            new SourceEvent
            {
                SourceName = inpatientTCI,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = bookedInpatientPreOperativeAssessment, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = inpatientTCIAgreed, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = bookedInpatientPreOperativeAssessment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = inpatientPreOperativeAssessment, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = patientAddedToInpatientWaitingList, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = inpatientPreOperativeAssessment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = inpatientPatientAttendancePreOperativeAssessment, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = inpatientPreOperativeAssessment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = patientCancelEvent, IsMandatory = false, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = hospitalCancelEvent, IsMandatory = false, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = didNotAttend, IsMandatory = false, ClockType = ClockType.ClockStopping}
                }
            },
            new SourceEvent
            {
                SourceName = inpatientPatientAttendancePreOperativeAssessment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = inpatientAdmitted, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = inpatientTCI, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = inpatientAdmitted,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = expectedInpatientDischarged, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = inpatientAdmitted, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = expectedInpatientDischarged,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = inpatientDischarged, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = expectedInpatientDischarged, ClockType = ClockType.ClockStopping},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = inpatientDischarged,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = episodeClinicallyCoded, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = inpatientDischarged, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = bookedPostInpatientFirstOutpatientAppointment, IsMandatory = false, TargetNumberOfDays = 14, EventForDateReferenceForTarget = inpatientDischarged, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = pathwayCompletion, IsMandatory = true,ClockType = ClockType.ClockStopping},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = episodeClinicallyCoded,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = bookedPostInpatientFirstOutpatientAppointment, IsMandatory = false, TargetNumberOfDays = 14, EventForDateReferenceForTarget = inpatientDischarged, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = bookedOutpatientFollowUpAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = bookedOutpatientFollowUpAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = outpatientFollowUpAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = attendedOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = outpatientFollowUpAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = patientCancelEvent, IsMandatory = false, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = hospitalCancelEvent, IsMandatory = false, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = didNotAttend, IsMandatory = false, ClockType = ClockType.ClockStopping}
                }
            },
            new SourceEvent
            {
                SourceName = attendedOutpatientFollowUpAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outcomedOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = attendedOutpatientFollowUpAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = outcomedOutpatientFollowUpAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outpatientDischarged, IsMandatory = true,ClockType = ClockType.ClockStopping},
                    new DestinationEvent {DestinationName = patientAddedToInpatientWaitingList, IsMandatory = true, TargetNumberOfDays = 10, EventForDateReferenceForTarget = outcomedOutpatientFirstAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = pathwayCompletion, IsMandatory = true,ClockType = ClockType.ClockStopping},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = bookedPostInpatientFirstOutpatientAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = postInpatientFirstOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = bookedPostInpatientFirstOutpatientAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = postInpatientFirstOutpatientFollowUpAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = attendedPostInpatientFirstOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = postInpatientFirstOutpatientFollowUpAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = patientCancelEvent, IsMandatory = false, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = hospitalCancelEvent, IsMandatory = false, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = didNotAttend, IsMandatory = false, ClockType = ClockType.ClockStopping}
                }
            },
            new SourceEvent
            {
                SourceName = attendedPostInpatientFirstOutpatientFollowUpAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outcomedPostInpatientFirstOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = attendedPostInpatientFirstOutpatientFollowUpAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = outcomedPostInpatientFirstOutpatientFollowUpAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outpatientDischarged, IsMandatory = true,ClockType = ClockType.ClockStopping},
                    new DestinationEvent {DestinationName = bookedPostInpatientSecondOutpatientAppointment, IsMandatory = false, TargetNumberOfDays = 14, EventForDateReferenceForTarget = outcomedPostInpatientFirstOutpatientFollowUpAppointment, ClockType = ClockType.None}, 
                    new DestinationEvent {DestinationName = patientAddedToInpatientWaitingList, IsMandatory = true, TargetNumberOfDays = 10, EventForDateReferenceForTarget = outcomedOutpatientFirstAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = pathwayCompletion, IsMandatory = true,ClockType = ClockType.ClockStopping},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = bookedPostInpatientSecondOutpatientAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = postInpatientSecondOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = bookedPostInpatientSecondOutpatientAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = postInpatientSecondOutpatientFollowUpAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = attendedPostInpatientSecondOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = postInpatientSecondOutpatientFollowUpAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = patientCancelEvent, IsMandatory = false, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = hospitalCancelEvent, IsMandatory = false, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = didNotAttend, IsMandatory = false, ClockType = ClockType.ClockStopping}
                }
            },
            new SourceEvent
            {
                SourceName = attendedPostInpatientSecondOutpatientFollowUpAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outcomedPostInpatientSecondOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = attendedPostInpatientSecondOutpatientFollowUpAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = outcomedPostInpatientSecondOutpatientFollowUpAppointment,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outpatientDischarged, IsMandatory = true,ClockType = ClockType.ClockStopping},
                    new DestinationEvent {DestinationName = patientAddedToInpatientWaitingList, IsMandatory = true, TargetNumberOfDays = 10, EventForDateReferenceForTarget = outcomedOutpatientFirstAppointment, ClockType = ClockType.None},
                    new DestinationEvent {DestinationName = pathwayCompletion, IsMandatory = true, ClockType = ClockType.ClockStopping},
                    new DestinationEvent {DestinationName = diagnosticsOrderPlaced, IsMandatory = false, TargetNumberOfDays = 42, EventForDateReferenceForTarget = referralReceived, ClockType = ClockType.None}
                }
            },
            new SourceEvent
            {
                SourceName = pathwayCompletion,
                NextPossibleEvents = new DestinationEvent[] { }
            },
            new SourceEvent
            {
                SourceName = patientCancelEvent,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outpatientFirstAppointment, IsMandatory = true, TargetNumberOfDays = 10, EventForDateReferenceForTarget = referralReceived},
                    new DestinationEvent {DestinationName = outpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = bookedOutpatientFollowUpAppointment},
                    new DestinationEvent {DestinationName = inpatientTCIAgreed, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = inpatientTCIOffer}, 
                    new DestinationEvent {DestinationName = inpatientPreOperativeAssessment, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = patientAddedToInpatientWaitingList},
                    new DestinationEvent {DestinationName = postInpatientFirstOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = bookedPostInpatientFirstOutpatientAppointment},
                    new DestinationEvent {DestinationName = postInpatientSecondOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = bookedPostInpatientSecondOutpatientAppointment}
                }
            },
            new SourceEvent
            {
                SourceName = hospitalCancelEvent,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outpatientFirstAppointment, IsMandatory = true, TargetNumberOfDays = 10, EventForDateReferenceForTarget = referralReceived},
                    new DestinationEvent {DestinationName = outpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = bookedOutpatientFollowUpAppointment},
                    new DestinationEvent {DestinationName = inpatientTCIAgreed, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = inpatientTCIOffer}, 
                    new DestinationEvent {DestinationName = inpatientPreOperativeAssessment, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = patientAddedToInpatientWaitingList},
                    new DestinationEvent {DestinationName = postInpatientFirstOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = bookedPostInpatientFirstOutpatientAppointment},
                    new DestinationEvent {DestinationName = postInpatientSecondOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = bookedPostInpatientSecondOutpatientAppointment}
                }
            },
            new SourceEvent
            {
                SourceName = didNotAttend,
                NextPossibleEvents = new[]
                {
                    new DestinationEvent {DestinationName = outpatientFirstAppointment, IsMandatory = true, TargetNumberOfDays = 10, EventForDateReferenceForTarget = referralReceived},
                    new DestinationEvent {DestinationName = outpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = bookedOutpatientFollowUpAppointment},
                    new DestinationEvent {DestinationName = inpatientTCIAgreed, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = inpatientTCIOffer}, 
                    new DestinationEvent {DestinationName = inpatientPreOperativeAssessment, IsMandatory = true, TargetNumberOfDays = 5, EventForDateReferenceForTarget = patientAddedToInpatientWaitingList},
                    new DestinationEvent {DestinationName = postInpatientFirstOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = bookedPostInpatientFirstOutpatientAppointment},
                    new DestinationEvent {DestinationName = postInpatientSecondOutpatientFollowUpAppointment, IsMandatory = true, TargetNumberOfDays = 0, EventForDateReferenceForTarget = bookedPostInpatientSecondOutpatientAppointment}
                }
            });
        }

        private void SeedUsersRolesAndPermissions(UnitOfWork context)
        {
            var permission1 = new Permission { Id = PermissionId.ManageUsers, Name = "Manage Users" };
            var permission2 = new Permission { Id = PermissionId.EditRolesActivitiesMapping, Name = "Edit Roles-Permissions Mapping" };
            var permission3 = new Permission { Id = PermissionId.ManageEventMilestones, Name = "Manage Event Milestones" };
            var permission4 = new Permission { Id = PermissionId.Patient, Name = "Patient Information" };
            var permission5 = new Permission { Id = PermissionId.EventBreaches, Name = "Event Breaches Information" };
            var permission6 = new Permission { Id = PermissionId.RTTPeriodBreaches, Name = "RTT Period Breaches Information" };
            var permission7 = new Permission { Id = PermissionId.MonthlyRTTPeriodPerformance, Name = "Monthly RTT Period Performance Information" };
            var permission8 = new Permission { Id = PermissionId.FuturePeriodBreaches, Name = "Future Period Breaches Information" };
            var permission9 = new Permission { Id = PermissionId.ActivePeriodsDistribution, Name = "Active Periods Distribution Information" };
            var permission10 = new Permission { Id = PermissionId.ActionsNotifications, Name = "Actions Notifications" };
            var permission11 = new Permission { Id = PermissionId.BreachesNotifications, Name = "Breaches Notifications" };
            var permission12 = new Permission { Id = PermissionId.RuleViolationsNotifications, Name = "Rule Violations Notifications" };
            var permission13 = new Permission { Id = PermissionId.ImportsNotifications, Name = "Imports Notifications" };
            var permission14 = new Permission { Id = PermissionId.Trust, Name = "Trust Information" };

            var role1 = new Role { Name = "Admin", Permissions = new[] { permission1, permission2, permission3, permission4, permission5, permission6, permission7, permission8, permission9, permission10, permission11, permission12, permission13, permission14 } };
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
            permission10.Roles = new[] { role1 };
            permission11.Roles = new[] { role1 };
            permission12.Roles = new[] { role1 };
            permission13.Roles = new[] { role1 };
            permission14.Roles = new[] { role1 };

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
