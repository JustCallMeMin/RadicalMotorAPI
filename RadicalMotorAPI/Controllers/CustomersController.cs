using Microsoft.AspNetCore.Mvc;
using RadicalMotor.Models;
using RadicalMotorAPI.DTO;
using RadicalMotorAPI.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
	private readonly ICustomerRepository _customerRepository;

	public CustomersController(ICustomerRepository customerRepository)
	{
		_customerRepository = customerRepository;
	}

	// POST: /Customers
	[HttpPost]
	public async Task<ActionResult<Customer>> PostCustomer([FromBody] CustomerDTO customerDto)
	{
		// Here we need to decide whether to check for an existing customer before adding a new one
		// For now, I'll assume that the IdentityCard is unique and we'll directly add the customer
		var customer = new Customer
		{
			IdentityCard = customerDto.IdentityCard,
			FullName = customerDto.FullName,
			DateOfBirth = customerDto.DateOfBirth,
			Address = customerDto.Address,
			Gender = customerDto.Gender,
			PhoneNumber = customerDto.PhoneNumber
			// The Accounts list initialization has been removed since it should be handled by business logic not directly in the controller
		};

		await _customerRepository.AddAsync(customer);
		return CreatedAtAction(nameof(GetCustomer), new { id = customer.IdentityCard }, customer);
	}

	// GET: /Customers
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
	{
		return Ok(await _customerRepository.GetAllAsync());
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Customer>> GetCustomer(string id)
	{
		var customer = await _customerRepository.GetByIdAsync(id);
		if (customer == null)
		{
			return NotFound();
		}
		return customer;
	}

	// PUT: /Customers/{id}
	[HttpPut("{id}")]
	public async Task<IActionResult> PutCustomer(string id, [FromBody] CustomerDTO customerDto)
	{
		var customer = await _customerRepository.GetByIdAsync(id);
		if (customer == null)
		{
			return NotFound();
		}

		customer.FullName = customerDto.FullName;
		customer.DateOfBirth = customerDto.DateOfBirth;
		customer.Address = customerDto.Address;
		customer.Gender = customerDto.Gender;
		customer.PhoneNumber = customerDto.PhoneNumber;

		await _customerRepository.UpdateAsync(customer);
		return NoContent();
	}

	// DELETE: /Customers/{id}
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteCustomer(string id)
	{
		await _customerRepository.DeleteAsync(id);
		return NoContent();
	}
}
