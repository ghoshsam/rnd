using Dapper.Data;
using DataAccessWithDapper;
using System;
using System.Collections.Generic;
using System.Text;


namespace Console.Dapper.Test
{
    public class CustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Customer> _customerRepository;
        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _customerRepository = _unitOfWork.CreateRepository<Customer>();
            
        }
    }
}
