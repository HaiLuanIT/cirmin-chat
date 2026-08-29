using FluentValidation;
using FluentValidation.TestHelper;
using Moji.Contracts.Models.Users.UpdateUserInfo;

namespace Moji.Contracts.Tests.Users.UpdateUserInfo;

public class UpdateUserInfoRequestValidatorTests
{
    private readonly IValidator<UpdateUserInfoRequest> _validator
        = new UpdateUserInfoRequestValidator();

    [Fact]
    public void Validate_DisplayNameExceedMaxLength_HasValidationError()
    {
        //arrange
        var updateUserInfoRequest = new UpdateUserInfoRequest
        {
            DisplayName = new string('a', 101)
        };

        //act
        var result = _validator.TestValidate(updateUserInfoRequest);


        //assert
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact]
    public void Validate_EmailInvalid_HasValidationError()
    {
        var updateUserInfoRequest = new UpdateUserInfoRequest
        {
            Email = "invalidEmail.com"
        };

        var result = _validator.TestValidate(updateUserInfoRequest);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_BioExceedMaxLength_HasValidationError()
    {
        var updateUserInfoRequest = new UpdateUserInfoRequest
        {
            Bio = new string('a', 102)
        };

        var result = _validator.TestValidate(updateUserInfoRequest);
        result.ShouldHaveValidationErrorFor(x => x.Bio);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(100, false)]
    [InlineData(101, true)]
    public void Validate_DisplayNameLength_EnforcesMaximumLength(int displayNameLenght, bool shouldHaveValidationError)
    {
        var updateUserInfoRequest = new UpdateUserInfoRequest
        {
            DisplayName = new string('a', displayNameLenght)
        };

        var result = _validator.TestValidate(updateUserInfoRequest);
        if (!shouldHaveValidationError)
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        else
            result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(101, false)]
    [InlineData(102, true)]
    public void Validate_BioLength_EnforcesMaximumLength(int bioLenght, bool shouldHaveValidationError)
    {
        var updateUserInfoRequest = new UpdateUserInfoRequest
        {
            Bio = new string('a', bioLenght)
        };
        var result = _validator.TestValidate(updateUserInfoRequest);

        if (shouldHaveValidationError)
            result.ShouldHaveValidationErrorFor(x => x.Bio);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.Bio);
    }

    [Theory]
    [InlineData(40, false)]
    [InlineData(41, true)]
    public void Validate_EmailLength_EnforcesMaximumLength(int emailLenght, bool shouldHaveValidationError)
    {
        var updateUserInfoRequest = new UpdateUserInfoRequest
        {
            Email = new string('a', emailLenght) + "@gmail.com"
        };
        var result = _validator.TestValidate(updateUserInfoRequest);
        if (shouldHaveValidationError)
            result.ShouldHaveValidationErrorFor(x => x.Email);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_NoOpRequest_HasNoValidationErrors()
    {
        var updateUserInfoRequest = new UpdateUserInfoRequest();
        var result = _validator.TestValidate(updateUserInfoRequest);
        result.ShouldNotHaveAnyValidationErrors();
    }
}