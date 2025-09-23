using FluentValidation;
using learnyx.Models.Requests;

namespace learnyx.Validators;

public class CourseSectionRequestValidator : AbstractValidator<CourseSectionRequest>
{
    public CourseSectionRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Section title is required.")
            .MaximumLength(200).WithMessage("Section title must not exceed 200 characters.");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Section order must be a positive number.");

        RuleForEach(x => x.Lessons)
            .SetValidator(new LessonRequestValidator());
    }
}