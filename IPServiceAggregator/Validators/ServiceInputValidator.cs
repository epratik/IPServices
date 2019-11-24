using FluentValidation;
using IPServiceAggregator.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IPServiceAggregator.Validators
{
    public class ServiceInputValidator: AbstractValidator<ServiceInput>
    {
        IConfiguration config;
        public ServiceInputValidator(IConfiguration config)
        {
            this.config = config;
            RuleFor(x => x.IpAddress).NotNull();
            RuleFor(x => x.IpAddress).Must(y => CheckIP(y))
                .WithMessage("Invalid IP address.");
            RuleFor(x => x.Services).Must(y => ValidateServices(y)).When(x => x.Services != null)
                .WithMessage("Incorrect service. Supported services are - " + config["defaultServices"]);
        }

        private bool CheckIP(string ip)
        {
            IPAddress ipAdd;
            return (ip.Split('.').Count() == 4 && IPAddress.TryParse(ip, out ipAdd));
        }

        private bool ValidateServices(string inpServices)
        {
            var inpSerArray = inpServices.Split(',');
            var availableSer = config["defaultServices"].Split(',');
            return inpSerArray.All(x => availableSer.Contains(x.ToLower()));
        }
    }
}
