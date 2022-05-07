﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyRabbitMQWeb.ExcelCreate.Models;
using UdemyRabbitMQWeb.ExcelCreate.Services;

namespace UdemyRabbitMQWeb.ExcelCreate.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public ProductController(AppDbContext context, UserManager<IdentityUser> userManager, RabbitMQPublisher rabbitMQPublisher)
        {
            _context = context;
            _userManager = userManager;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProductExcel()
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";
            UserFile userFile = new UserFile()
            {
                UserId = user.Id,
                FileName = fileName,
                FileStatus = FileStatus.Creating
            };

            await _context.UserFile.AddAsync(userFile);
            await _context.SaveChangesAsync();
            _rabbitMQPublisher.Publish(new Shared.CreateExcelMessage()
            {
                FileId = userFile.Id
            });

            TempData["StartCreatingExcel"] = true;

            return RedirectToAction(nameof(File));
        }

        public async Task<IActionResult> File()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userFile = await _context.UserFile.Where(x => x.UserId == user.Id).OrderByDescending(x=>x.Id).ToListAsync();
            return View(userFile);
        }
    }
}
