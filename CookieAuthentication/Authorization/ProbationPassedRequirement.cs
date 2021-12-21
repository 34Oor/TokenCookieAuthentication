using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace CookieAuthentication.Authorization
{
    public class ProbationPassedRequirement : IAuthorizationRequirement
    {
        public ProbationPassedRequirement(int probationPeriod)
        {
            ProbationPeriod = probationPeriod;  
        }
        public int ProbationPeriod { get; set; }
    }

    public class ProbationPassedRequirmentHandler : AuthorizationHandler<ProbationPassedRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProbationPassedRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "EmploymentDate"))
                return Task.CompletedTask;
            
            var workingPeriod = DateTime.Now - DateTime.Parse(context.User.FindFirst(c => c.Type == "EmploymentDate").Value);

            if (workingPeriod > TimeSpan.FromDays(requirement.ProbationPeriod * 30))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
