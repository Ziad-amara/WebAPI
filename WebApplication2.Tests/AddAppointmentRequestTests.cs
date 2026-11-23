using System.ComponentModel.DataAnnotations;
using WebApplication2.Dtos.AppointmentDtos;

namespace WebApplication2.Tests;

public class AddAppointmentRequestTests
{
    [Fact]
    public void Validation_FailsWhenPatientOrDoctorIdIsMissing()
    {
        var request = new AddAppointmentRequest
        {
            AppointmentDate = new DateTime(2026, 11, 23)
        };
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            validationResults,
            validateAllProperties: true);

        Assert.False(isValid);
        Assert.Contains(validationResults, result => result.MemberNames.Contains(nameof(request.PatientId)));
        Assert.Contains(validationResults, result => result.MemberNames.Contains(nameof(request.DoctorId)));
    }

    [Fact]
    public void Validation_PassesWhenRequiredIdsAreProvided()
    {
        var request = new AddAppointmentRequest
        {
            PatientId = 1,
            DoctorId = 2,
            AppointmentDate = new DateTime(2026, 11, 23, 10, 0, 0)
        };
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            validationResults,
            validateAllProperties: true);

        Assert.True(isValid);
        Assert.Empty(validationResults);
    }
}
