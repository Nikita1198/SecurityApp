using Microsoft.AspNetCore.Authorization;
using System;

namespace SecurityApp.Authorization
{
    public class HRManagerProbationRequirment : IAuthorizationRequirement
    {
        public HRManagerProbationRequirment(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }

        public int ProbationMonths { get; }
    }

    public class HRManagerProbationRequirmentHandler : AuthorizationHandler<HRManagerProbationRequirment>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirment requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "EmploymentDate"))
                return Task.CompletedTask;

            var empData = DateTime.Parse(context.User.FindFirst(x => x.Type == "EmploymentDate").Value);
            var period = DateTime.Now - empData;
            if (period.Days > 30 * requirement.ProbationMonths)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
