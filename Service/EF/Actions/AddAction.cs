using Microsoft.AspNetCore.Builder;
using System;

namespace EF.Actions
{
    public class AddAction 
    {
        public int Priority => 3000;
        public void Excute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {

        }
    }
}
