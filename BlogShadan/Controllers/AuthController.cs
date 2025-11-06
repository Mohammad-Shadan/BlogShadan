using BlogShadan.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogShadan.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        //Register
        //Login
        //Logout
        public AuthController(UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager=signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            //Check for Validation
            if (ModelState.IsValid)
            {
                //Create Identity User object
                var user = new IdentityUser { UserName=model.Email,Email=model.Email };
                //User created
                var result = await _userManager.CreateAsync(user,model.Password);

                //Check if user created successfully or not
                if (result.Succeeded) {

                    //Checking if User role exist in database or not
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        //If not exist then add the role
                        await _roleManager.CreateAsync( new IdentityRole("User"));
                    }

                    //Assigning User role to the new member registered
                    await _userManager.AddToRoleAsync(user, "User");

                    //Signing the regiuster User
                   await _signInManager.SignInAsync(user, isPersistent: true);

                    return RedirectToAction("Index", "Post");
                }

            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVIewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

            if(user == null)
                {
                    ModelState.AddModelError("", "Email or Password is Incorrect");
                    return View(model);
                }
                var signInresult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (!signInresult.Succeeded)
                {
                    ModelState.AddModelError("", "Email or Password is Incorrect");
                    return View(model);
                }

                return RedirectToAction("Index", "Post");

            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Post");
        }

    }
}
