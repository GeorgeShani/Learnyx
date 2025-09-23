using FluentValidation;
using learnyx.Models.Requests;

namespace learnyx.Validators;

public class LessonRequestValidator : AbstractValidator<LessonRequest>
{
    public LessonRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Lesson title is required.")
            .MaximumLength(200).WithMessage("Lesson title must not exceed 200 characters.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid lesson type.");

        RuleFor(x => x.Duration)
            .Matches(@"^\d{1,2}:\d{2}$").WithMessage("Duration must be in format mm:ss or hh:mm.")
            .When(x => !string.IsNullOrEmpty(x.Duration));

        RuleFor(x => x.Content)
            .MaximumLength(5000).WithMessage("Lesson content must not exceed 5000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Content));

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Lesson order must be a positive number.");

        RuleForEach(x => x.Resources)
            .NotEmpty().WithMessage("Resources cannot contain empty values.")
            .MaximumLength(300).WithMessage("Resource link must not exceed 300 characters.");
    }   
}