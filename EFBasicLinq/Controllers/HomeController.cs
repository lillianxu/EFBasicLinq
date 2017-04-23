using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFBasicLinq.Models;
using EFBasicLinq;
using System.Data.Entity.Infrastructure;

namespace EFBasicLinq.Controllers
{
    public class HomeController : Controller
    {

        SchoolEntities db = new SchoolEntities();

        #region query
        public ActionResult Index()
        {
            //1 query all course-SQO
            //return iqueryable 
            //IQueryable<Models.Staff> query= db.Courses.Where(c=>c.CourseID!=0);
            //EnumerableQuery<Models.Staff> query= (EnumerableQuery<Models.Staff>)db.Courses.Where(c=>c.CourseID!=0)as EnumerableQuery<Models.Staff>;
            //query 得到的值是null，真实的返回类型是DbQuery<T>,支持延迟加载
            //将返回的DbQuery转成List<T>集合，立即查询数据库，并返回查询到的值
            //SQL profiler 跟踪查询有没有生成SQL语句
            //List<Models.StudentGrade> list = db.StudentGrades.Where(s => s.StudentID!= 0).ToList();//SQO
            List<Models.StudentGrade> list = (from s in db.StudentGrades where s.StudentID != 0 select s).ToList();//LINQ

            //2将集合数据传给试图
            //ViewBag.DataList = list;
            ViewData["Datalist"] = list;

            return View();
        }
        #endregion

        #region Del(id)
        public ActionResult Del(int id)//此id会根据路由的URl配置（id）占位符，而被相应12替换，参看RouteConfig.cs url
        {

            // db.StudentGrades.Remove(new StudentGrade() { EnrollmentID = id });//new 一个studentgrade 是因为要求传一个实体对象，所以必须把id装到这个表的实体对象里边去

            try {
                //1创建要删除的对象
                StudentGrade modelDel = new StudentGrade() { EnrollmentID = id };
                //2将对象添加到EF容器中
                db.StudentGrades.Attach(modelDel);
                //3将对象状态标识为删除状态
                db.StudentGrades.Remove(modelDel);
                //4更新到数据库
                db.SaveChanges();
                //5 更新成功，则命令浏览器重定向到/Home/Index方法
                return RedirectToAction("Index", "Home");
            }

            catch (Exception ex)
            {
                return Content("fail to del" + ex.Message);
            }

        }
        #endregion

        #region Edit
        [HttpGet]
        public ActionResult Edit(int id)
        {
            //1根据id查询数据库，返回的集合中拿到第一个实体对象
            StudentGrade sg = (from s in db.StudentGrades where s.EnrollmentID == id select s).FirstOrDefault();

            //生成dropdownlist列表集合
            IEnumerable<SelectListItem> listItem = (from c in db.Courses select c).ToList()
                                            .Select(c => new SelectListItem { Value = c.CourseID.ToString(), Text = c.Title });
            ViewBag.CourseList = listItem;
            //2将sg 传递给视图
            //ViewBag
            //ViewData
            //使用View的构造函数，将数据传给视图上的名为Model的属性
            return View(sg);
        }
        
        //执行修改
        [HttpPost]
        public ActionResult Edit(StudentGrade model)
        {

            try { 
            //将实体对象加入EF对象容器中，并获取伪包装类
            DbEntityEntry<StudentGrade> entry= db.Entry<StudentGrade>(model);
            //将包装类对象的状态设置为unchanged
            entry.State = System.Data.EntityState.Unchaged;
            //设置被改变的属性
            entry.Property(a => a.StudentID).IsModified = true;
            entry.Property(a => a.CourseID).IsModified = true;
            entry.Property(a => a.Grade).IsModified = true;
            //提交到数据库
            db.SaveChanges();
                //更新成功，重定向
            return RedirectToAction("Index", "Home");

            }
            catch(Exception ex)
            {
                return Content("Fail"+ex.Message);
            }
        }
        #endregion

        #region
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        #endregion Default

       


    }
}