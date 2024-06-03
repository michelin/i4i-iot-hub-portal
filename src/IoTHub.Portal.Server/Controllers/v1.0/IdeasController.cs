// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace IoTHub.Portal.Server.Controllers.v1._0
{
    using System.Threading.Tasks;
    using IoTHub.Portal.Application.Services;
    //using IoTHub.Portal.Shared.Security;
    //using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Shared.Models.v1._0;

    //[Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/ideas")]
    [ApiExplorerSettings(GroupName = "Ideas")]
    public class IdeasController : ControllerBase
    {
        private readonly IIdeaService ideasService;

        public IdeasController(IIdeaService ideasService)
        {
            this.ideasService = ideasService;
        }

        [HttpPost(Name = "Submit Idea to Iot Hub Portal community")]
        //[Authorize(Policy = Policies.SumitIdea)]
        public Task<IdeaResponse> SubmitIdea([FromBody] IdeaRequest ideaRequest)
        {
            return this.ideasService.SubmitIdea(ideaRequest, Request.Headers.UserAgent.ToString());
        }
    }
}
