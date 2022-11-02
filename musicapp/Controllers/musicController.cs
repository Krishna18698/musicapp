using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using musicapp.Models;
using System.Data;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Policy;
using System.Runtime.ConstrainedExecution;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Hosting;

namespace musicapp.Controllers
{
   
    public class musicController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public musicController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        music1Context dc = new music1Context();
        public IActionResult Index()
        {
            return View();
        }
     

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User r)
        {
            Console.WriteLine(r.Username);

            var res = (from t in dc.Users
                       where t.Username == r.Username && t.Password == r.Password
                       select t).FirstOrDefault();

            if (res != null)
            {

                List<Claim> li = new List<Claim>()
                {
                  new Claim(ClaimTypes.Name,r.Username),
                   new Claim(ClaimTypes.Role,res.Role),
                   new Claim(ClaimTypes.Sid,res.Email)
               };
                var identity = new ClaimsIdentity(li, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties() { IsPersistent = true });

                string role = User.FindFirstValue(ClaimTypes.Role);
                if (res.Role == "Admin")
                {
                    return RedirectToAction("AdminControls");
                }



                return RedirectToAction("dashboard");
            }
            else
            {
                ViewData["error"] = "invalid uname and password!!!";
            }


            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Songlist()
        {
            var res = from t in dc.Musics
                      select t;
            return View(res);
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(User u)
        {

            var acc = dc.BankAccounts.Where(a => a.Email == u.Email).FirstOrDefault();
            dc.Users.Add(u);

            BankAccount bankaccount = new BankAccount()
            {
                Email = u.Email,
                Balance = 100000
            };
            dc.BankAccounts.Add(bankaccount);

            int i=dc.SaveChanges();

            if (i > 0)
            {
                ViewData["msg"] = "new user created successfully!!";
                decimal plan = int.Parse(u.MusicPlan);

                bankaccount.Balance -= plan;
                dc.BankAccounts.Update(bankaccount);
                
                Payment pay = new Payment()
                {
                    Email = u.Email,
                    Paymentamt = plan
                };
                dc.Payments.Add(pay);
                dc.SaveChanges();
            }
            else ViewData["msg"] = "account creation unsuccessfull";

            return RedirectToAction("Login");
        }

        public IActionResult Home()
        {
            var res = from t in dc.Musics
                      where t.MusicId == 1 || t.MusicId == 4 || t.MusicId == 6
                      select t;
            return View(res);

        }

        public IActionResult Music()
        {
            return View();
        }

        [Authorize(Roles = "Admin,User")]
        public IActionResult dashboard()
        {

            return View();
        }

        public IActionResult classical()
        {
            var res = from t in dc.Musics
                      where t.Genrename == "Classical" && t.Genreid == 2
                      select t;
            return View(res);

        }


        public IActionResult western()
        {
            var res = from t in dc.Musics
                      where t.Genrename == "western" && t.Genreid == 1
                      select t;
            return View(res);

        }

        public IActionResult rock()
        {
            var res = from t in dc.Musics
                      where t.Genrename == "Rock" && t.Genreid == 3
                      select t;
            return View(res);
        }


        public IActionResult instrumental()
        {
            var res = from t in dc.Musics
                      where t.Genrename == "Instrumental" && t.Genreid == 4
                      select t;
            return View(res);
        }



        public IActionResult devotional()
        {
            var res = from t in dc.Musics
                      where t.Genrename == "devotional" && t.Genreid == 5
                      select t;
            return View(res);

        }
        public IActionResult Edm()
        {
            var res = from t in dc.Musics
                      where t.Genrename == "Edm" && t.Genreid == 9
                      select t;
            return View(res);
        }



        [Authorize(Roles = "Admin")]
        public IActionResult addsongs()
        {
            return View();
        }

        [HttpPost]
        public  async Task<IActionResult> addsongs(Music m)
        {

            
                if (m.imgpath != null)
                {
                    string folder = "image/";
                      m.ImgPath =await  imageupload(folder, m.imgpath);

                 }
            if (m.musicpath != null)
            {
                string folder = "image/";
                m.MusicPath = await imageupload(folder, m.musicpath);

            }



            await dc.Musics.AddAsync(m);
            await dc.SaveChangesAsync();
            ViewData["msg"] = "new song added successfully!!";
            return RedirectToAction("Songlist");
        }
        public async Task<string> imageupload(string folderPath, IFormFile file)
        {
            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;
            string severFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);
            await file.CopyToAsync(new FileStream(severFolder, FileMode.Create));
            return "/" + folderPath;
        }

            [Authorize(Roles = "Admin")]
        public IActionResult addGenres()
        {
            return View();
        }

        [HttpPost]
        public IActionResult addGenres(Genre g)
        {
            dc.Genres.Add(g);
            dc.SaveChanges();
            ViewData["msg"] = "new Genre added successfully!!";
            return View();
        }





        [Authorize(Roles = "Admin")]
        public IActionResult Userlist()
        {
            var res = from t in dc.Users
                      from f in dc.Payments
                      where t.Username!="Admin" && t.Email==f.Email
                      select new Userlists { user=t,payment=f}
                      ;
            return View(res);

        }
        public IActionResult UserDelete()
        {
            string s = Request.Query["username"];
            var res = dc.Users.Where(t => t.Username == s).FirstOrDefault();
            if (res != null)
            {
                dc.Users.Remove(res);
                dc.SaveChanges();
                ViewData["msg"] = "Deleted successfully";
            }
            else
            {
                ViewData["msg"] = "Not Deleted!";
            }
            return RedirectToAction("UserList");
        }

        public IActionResult Delete()
        {
            int s = Convert.ToInt32(Request.Query["music_id"]);
            var res = dc.Musics.Where(t => t.MusicId == s).FirstOrDefault();





            if (res != null)
            {
                dc.Musics.Remove(res);
                dc.SaveChanges();
                ViewData["msg"] = "Deleted successfully";
            }
            else
            {
                ViewData["msg"] = "Not Deleted!";
            }





            return RedirectToAction("SongList");
        }


        public IActionResult feedback()
        {

            ViewData["email"] = Request.Query["email"];
            ViewData["music_name"] = Request.Query["music_name"];

            return View();
        }
        [HttpPost]
        public IActionResult feedback(Feedback1 ord)
        {

            Feedback1 feed = new Feedback1()
            {
                Email = User.FindFirstValue(ClaimTypes.Sid),
                Songname = ord.Songname,
            };


            ord.Songname = Request.Query["music_name"];
            ord.Email = User.FindFirstValue(ClaimTypes.Sid);

            dc.Add(ord);
            dc.SaveChanges();
            return RedirectToAction("dashboard");

        }
        [Authorize(Roles = "Admin")]
        public IActionResult coments()
        {
            var res = from t in dc.Feedback1s
                      select t;
            return View(res);
        }


        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");

        }

     

        public IActionResult AdminControls()
        {
            return View();

        }
        [Authorize(Roles = "Admin")]
        public IActionResult Genrelist()
        {
            var res = from t in dc.Genres
                      select t;
            return View(res);

        }
        [Authorize(Roles = "Admin")]
        public IActionResult GenreDelete()
        {
            string s = Request.Query["genrename"];
            var res = dc.Genres.Where(t => t.Genrename == s).FirstOrDefault();
            if (res != null)
            {
                dc.Genres.Remove(res);
                dc.SaveChanges();
                ViewData["msg"] = "Deleted successfully";
            }
            else
            {
                ViewData["msg"] = "Not Deleted!";
            }
            return RedirectToAction("Genrelist");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AddGenre()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddGenre(Genre g)
        {
            dc.Genres.Add(g);
            dc.SaveChanges();
            ViewData["msg"] = "new song added successfully!!";
            return View();
        }
        public IActionResult contactus()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public IActionResult Search(string ser)
        {


            var res = from t in dc.Musics
                      where t.MusicName == ser
                      select t;




            return View(res);
        }
        //public IActionResult Update()
        //{
        //    return View();
        //}

        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //public IActionResult Update(Music m)
        //{
        //    int i = Convert.ToInt32(Request.Query["MusicId"]);
        //    var upd = dc.Musics.FirstOrDefault(t => t.MusicId == i);
        //    if (upd != null)
        //    {
        //        upd.Genreid = m.Genreid;
        //        upd.Genrename = m.Genrename;
        //        upd.MusicId = m.MusicId;
        //        upd.MusicName = m.MusicName;
        //        upd.MusicPath = m.MusicPath;
        //        upd.ImgPath = m.ImgPath;
        //    }
        //    else
        //    {
        //        ViewData["i"] = "null";
        //    }
        //    dc.Musics.Update(upd);
        //    dc.SaveChanges();

            //if (id != 0)
            //{
            //    TempData["accept"] = "Job requirement accepted.";
            //}



        //    return RedirectToAction("Songlist");
        //}

    }
}
