# Simple Netcore MVC EF with docker-compose , AdminLTE and connect Postgres database

1. Install Netcore and AdminLTE :  https://github.com/labfix/netcore-adminlte

2. Install "Postgres" database from dockerr-compose : https://github.com/labfix/postgres

and Restored "northwind.sql" file from project

### Install Package 
```
$ cd LabFix.NetcoreMVC
$ dotnet add . package Microsoft.AspNetCore.Mvc.TagHelpers
$ dotnet add . package Microsoft.EntityFrameworkCore
$ dotnet add . package NewtonSoft.Json
$ dotnet add . package Npgsql.EntityFrameworkCore.PostgreSQL
$ dotnet add . package Npgsql.EntityFrameworkCore.PostgreSQL.Design
$ dotnet add . package Microsoft.EntityFrameworkCore.Tools.DotNet

$ dotnet restore
```
if can't build and error. please change 
error NU1605: Detected package downgrade: Microsoft.NETCore.App from 2.0.6 to 2.0.0. Reference the package directly from the project to select a different version.
error NU1605:  LabFix.NetcoreMVC (>= 1.0.0) -> Microsoft.EntityFrameworkCore.Tools.DotNet (>= 2.0.2) -> Microsoft.NETCore.App (>= 2.0.6)
error NU1605:  LabFix.NetcoreMVC (>= 1.0.0) -> Microsoft.NETCore.App (>= 2.0.0)

```
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.2" />

Change to

<DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0" />
```

### Initial and create Models via EF

Go to root project "LabFix.NetcoreMVC" and run
```
$ dotnet ef dbcontext scaffold "Host=localhost;Database=northwind;Username=labfix;Password=1234" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -f

$ dotnet build
```
if error Cannot implicitly convert type 'string' to 'char' , database colume is "character". please change colume type to "character varying".

### Add connection
1. create file 'appsettings.json' on root folder
Add line :
```
{
    "ConnectionStrings" : {
        "PostgressConnection": "User ID=labfix;Password=1234;Host=localhost;Port=5432;Database=northwind;Pooling=true;"
    }
} 
```
2. edit "Startup.cs" Addline
```
using LabFix.NetcoreMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

```
        public Startup(IConfiguration configuration){
            Configuration = configuration;
        }

        public IConfiguration Configuration {get;}
```
```
And

```

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //Add this
            var pgConnectionString = Configuration.GetConnectionString("PostgressConnection");
            services.AddDbContext<northwindContext>(options => options.UseNpgsql(pgConnectionString));
        }
```

3. edit Models/northwindContext.cs
Add line :
```
 public northwindContext(DbContextOptions<northwindContext> options): base(options){}
```
and comment code :
```
//         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//         {
//             if (!optionsBuilder.IsConfigured)
//             {
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                 optionsBuilder.UseNpgsql(@"Host=localhost;Database=northwind;Username=labfix;Password=1234");
//             }
//         }

```

### Create new page
edit file Startup.cs , function Route Add line :
```
           app.UseMvc(routes=>{
......
......
               routes.MapRoute(
                   name:"manage",
                   template:"{controller=Manage}/{action=Index}/{id?}"
               );
......
           });
```

### This project don't have set env. please comment tag <environment> in _Layout.cshtml
 ```
 <environment include="Development">
 .....
 </environment>
```

### Set Main Layout and extent Microsoft.AspNetCore.Mvc.TagHelpers
1. Create Views/_ViewImports.cshtml
```
@using LabFix.NetcoreMVC
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

2. Create Views/_ViewStart.cshtml
```
@{
    Layout = "_Layout";
}
```
3. Remove Tag Layout = "_Layout"; from view page. becuese we use _ViewStart.cshtml.

### Create file
/Controllers/ManageController.cs
/Views/Manage/Create.cshtml
/Views/Manage/Edit.cshtml
/Views/Manage/Customer.cshtml

#### ManageController.cs
```
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LabFix.NetcoreMVC.Models;
using System.Linq;
using System.Text;
using System;

namespace LabFix.NetcoreMVC.Controllers
{
    public class ManageController : Controller
    {
        private northwindContext _dbContext;
        public ManageController(northwindContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Customer()
        {

            var cusAll = _dbContext.Customers.ToList();

            return View(cusAll);

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        public string GenerateAutoId(int length)
        {
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        [HttpPost]
        public IActionResult Create(Customers customers)
        {
            if (ModelState.IsValid)
            {
                customers.Customerid = GenerateAutoId(4);
                _dbContext.Customers.Add(customers);
                _dbContext.SaveChanges();

                return RedirectToAction("Customer");
            }

            return View(customers);
        }


        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            else
            {
                var data = _dbContext.Customers.FirstOrDefault(x => x.Customerid == id);
                return View(data);
            }
        }

        [HttpPost]
        public IActionResult Edit(
            string id,
            [Bind("Customerid,Companyname,Contactname,Contacttitle,Country,Postalcode")] Customers customers)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _dbContext.Customers.Update(customers);
                _dbContext.SaveChanges();

                return RedirectToAction("Customer");
            }
            return View(customers);
        }

        public IActionResult Delete(string id)
        {
            var cust = _dbContext.Customers.FirstOrDefault(m => m.Customerid == id);
            if (cust == null)
            {
                return RedirectToAction("Customer");
            }
            _dbContext.Customers.Remove(cust);
            _dbContext.SaveChanges();
            return RedirectToAction("Customer");
        }

        public IActionResult Error()
        {
            return View();
        }
    }

}
```

#### Customer.cshtml
```
@model IEnumerable<LabFix.NetcoreMVC.Models.Customers>

    <!-- Main content -->
    <section class="content">
      <div class="row">
        <div class="col-xs-12">
    
          <div class="box">
            <div class="box-header">
              <h3 class="box-title">Data Table With Full Features</h3>
            </div>
            <!-- /.box-header -->
            <div class="box-body"
            <br/>
              <a href="~/Manage/Create">Create New Customer</a>
              <br/>
              <h4>Persons List</h4>>
              <table id="example1" class="table table-bordered table-striped">
                <thead>
                <tr>
                  <th>Customerid</th>
                  <th>Companyname</th>
                  <th>Contactname</th>
                  <th>Contacttitle</th>
                  <th>Country</th>
                  <th>Postalcode</th>
                </tr>
                </thead>
                <tbody>
                @foreach(var item in Model)
                {
                <tr>
                  <td>@item.Customerid</td>
                  <td>@item.Companyname</td>
                  <td>@item.Contactname</td>
                  <td>@item.Contacttitle</td>
                  <td>@item.Country</td>
                  <td>@item.Postalcode</td>
                  <td><a asp-action="Edit" asp-route-id="@item.Customerid">Edit</a> |
                  <a asp-action="Delete" asp-route-id="@item.Customerid">Delete</a></td>
                </tr>
                }
              </table>
            </div>
            <!-- /.box-body -->
          </div>
          <!-- /.box -->
        </div>
        <!-- /.col -->
      </div>
      <!-- /.row -->
    </section>
    <!-- /.content -->
  
```

#### Create.cshtml
```
@model LabFix.NetcoreMVC.Models.Customers
 
 <!-- Helper Tags -->
<form asp-controller="Manage" asp-action="Create"  method="post" class="form-horizontal">
    <h4>Create a new Person.</h4>
    <hr />
    <!--div asp-validation-summary="All" class="text-danger"></div -->
    <div class="form-group">
        <!-- label asp-for="Companyname" class="col-md-2 control-label"></label-->
        <div class="col-md-10">
            <input asp-for="Companyname" class="form-control" />
            <!-- span asp-validation-for="Companyname" class="text-danger"></span -->
        </div>
    </div>
    <div class="form-group">
        <label asp-for="Address" class="col-md-2 control-label"></label>
        <div class="col-md-10">
            <input asp-for="Contactname" class="form-control" />
            <!-- span asp-validation-for="Contactname" class="text-danger"></span -->
        </div>
    </div>
     <div class="form-group">
        <label asp-for="Phone" class="col-md-2 control-label"></label>
        <div class="col-md-10">
            <input asp-for="Contacttitle" class="form-control" />
            <!-- span asp-validation-for="Contacttitle" class="text-danger"></span-->
        </div>
    </div>
    <div class="form-group">
        <div class="col-md-offset-2 col-md-10">
            <button type="submit" class="btn btn-default">Create</button>
        </div>
    </div>
</form>
 
<div>
    @Html.ActionLink("Back to List", "Customer")
</div>
```

#### Edit.cshtml
```
@model LabFix.NetcoreMVC.Models.Customers
<h2>Edit</h2>
 
 <!-- Normal Razor Tags Helper -->

@using (Html.BeginForm()) {
    @Html.ValidationSummary(true)
 
    <fieldset>
        <legend>Person</legend>
 
        @Html.HiddenFor(model => model.Customerid)
 
        <div class="editor-label">
            @Html.LabelFor(model => model.Companyname)
        </div>
        <div class="editor-field">
            @Html.EditorFor(model => model.Companyname)
        </div>
        <div class="editor-field">
            @Html.EditorFor(model => model.Contactname)
            //@Html.ValidationMessageFor(model => model.Contactname)
        </div>
 
        <div class="editor-label">
            @Html.LabelFor(model => model.Country)
        </div>
        <div class="editor-field">
            @Html.EditorFor(model => model.Country)
            //@Html.ValidationMessageFor(model => model.Country)
        </div>
 
        <div class="editor-label">
            @Html.LabelFor(model => model.Postalcode)
        </div>
        <div class="editor-field">
            @Html.EditorFor(model => model.Postalcode)
            //@Html.ValidationMessageFor(model => model.Postalcode)
        </div>
        <br/>
        <div>
            <p>
                <input type="submit"  class="btn btn-default" value="Save" />
            </p>
        </div>
    </fieldset>
}
 
<div>
    @Html.ActionLink("Back to List", "Customer")
</div>
```