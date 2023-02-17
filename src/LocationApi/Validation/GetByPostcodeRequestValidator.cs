using FluentValidation;
using LocationApi.Helpers;
using LocationApi.Requests;

namespace LocationApi.Validation
{
    public class GetByPostcodeRequestValidator : AbstractValidator<GetByPostcodeRequest>
    {
        public GetByPostcodeRequestValidator()
        {
            RuleFor(x => x.Postcode).NotEmpty();
            RuleFor(x => x.Postcode)
                .Must(x => PostcodeValidator.IsValid(PostcodeValidator.Sanitize(x.Trim().Replace(" ", ""))))
                .WithMessage("Postcode invalid, please enter a full postcode. e.g. AB123CD");
        }
    }
}