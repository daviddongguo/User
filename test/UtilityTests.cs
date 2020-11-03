using NUnit.Framework;
using System;
using user.Services;

namespace test
{
    public class UtilityTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateRandomIdTest()
        {
            string result;
            for (int i = 0; i < 10; i++)
            {
                result = Utility.RandomString(10);
                System.Console.WriteLine(result);
                Assert.That(result, Is.Not.Empty);
            }
        }

        [Test]
        public void CreatePasswordHasTest()
        {
            for (int i = 0; i < 10; i++)
            {
                Utility.CreatePasswordHash(i.ToString(), out byte[] passwordHash, out byte[] passwordSalt);
                System.Console.WriteLine($"{ i} : {Convert.ToBase64String(passwordHash).Substring(0,10)} : {Convert.ToBase64String(passwordSalt).Substring(0,10)}");
                Assert.That(passwordHash, Is.Not.Empty);
                Assert.That(passwordSalt, Is.Not.Empty);
            }

        }
    }
}
