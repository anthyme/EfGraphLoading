using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Anthyme.EfGraphLoading
{
    public class Subject
    {
        public virtual int Id { get; set; }
        public virtual string Content { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
    }

    public class Category
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ICollection<SubCategory> SubCategories { get; set; }
    }

    public class SubCategory
    {
        public virtual int Id { get; set; }
        public virtual int Name { get; set; }
        public virtual int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }

    public class GraphContext : DbContext
    {
        public GraphContext()
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
            Database.SetInitializer(new GraphContextInitializer());
        }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
    }

    public class GraphContextInitializer : DropCreateDatabaseIfModelChanges<GraphContext>
    {
        private static string lorem = string.Join("", Enumerable.Repeat("lorem ipsum ", 10000));

        protected override void Seed(GraphContext context)
        {
            base.Seed(context);

            foreach (var i in Enumerable.Range(0, 50))
            {
                var subject = context.Subjects.Create();
                subject.Content = lorem;

                foreach (var j in Enumerable.Range(0, 30))
                {
                    var category = context.Categories.Create();
                    category.Name = "Category " + i + " " +  j;
                    subject.Categories.Add(category);

                    foreach (var k in Enumerable.Range(0, 20))
                    {
                        var subCategory = context.SubCategories.Create();
                        category.Name = "SubCategory " + i + " " + j + " " + k;
                        category.SubCategories.Add(subCategory);
                    }
                }
                context.Subjects.Add(subject);
                context.SaveChanges();
            }

            //context.SaveChanges();
        }
    }
}
