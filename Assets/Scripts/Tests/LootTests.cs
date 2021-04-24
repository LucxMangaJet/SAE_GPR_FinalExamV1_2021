using NUnit.Framework;
using UnityEngine;

public class LootTests
{
    [Test]
    public void EmptyLootDescriptionTest()
    {
        var lootDescription = ScriptableObject.CreateInstance<LootDescription>();

        lootDescription.SetDrops();
        for (int i = 0; i < 100; i++)
        {
            var drop = lootDescription.SelectDropRandomly();
            Assert.AreEqual(null, drop);
        }

        Object.DestroyImmediate(lootDescription);
    }

    [Test]
    public void CertainDropLootDescriptionTest()
    {
        var lootDescription = ScriptableObject.CreateInstance<LootDescription>();
        var testDrop = ScriptableObject.CreateInstance<Drop>();
        testDrop.DropName = "TestDrop";

        lootDescription.SetDrops(new DropProbabilityPair[] {
            new DropProbabilityPair() { Drop = testDrop, Probability = 1 } });

        for (int i = 0; i < 100; i++)
        {
            var drop = lootDescription.SelectDropRandomly();
            Assert.AreEqual(testDrop, drop);
        }

        Object.DestroyImmediate(lootDescription);
        Object.DestroyImmediate(testDrop);
    }

    [Test]
    public void EnsureDropWith5050Distribution()
    {
        var lootDescription = ScriptableObject.CreateInstance<LootDescription>();
        var testDropA = ScriptableObject.CreateInstance<Drop>();
        testDropA.DropName = "a";

        var testDropB = ScriptableObject.CreateInstance<Drop>();
        testDropB.DropName = "b";

        lootDescription.SetDrops(new DropProbabilityPair[] {
            new DropProbabilityPair() { Drop = testDropA, Probability = 0.5f }, 
            new DropProbabilityPair(){Drop = testDropB, Probability = 0.5f } });

        for (int i = 0; i < 10000; i++)
        {
            var drop = lootDescription.SelectDropRandomly();
            Assert.NotNull(drop);
        }

        Object.DestroyImmediate(lootDescription);
        Object.DestroyImmediate(testDropA);
        Object.DestroyImmediate(testDropB);
    }

    [Test]
    public void EnsureCorrect8020DistributionResults()
    {
        var lootDescription = ScriptableObject.CreateInstance<LootDescription>();
        var testDropA = ScriptableObject.CreateInstance<Drop>();
        testDropA.DropName = "a";

        var testDropB = ScriptableObject.CreateInstance<Drop>();
        testDropB.DropName = "b";

        //Test 80-20 distribution
        lootDescription.SetDrops(new DropProbabilityPair[] {
            new DropProbabilityPair() { Drop = testDropA, Probability = 0.8f },
            new DropProbabilityPair(){Drop = testDropB, Probability = 0.2f } });
        int sumA = 0;
        int sumB = 0;

        for (int i = 0; i < 100000; i++)
        {
            var drop = lootDescription.SelectDropRandomly();
            if (drop == testDropA) sumA++;
            else if (drop == testDropB) sumB++;
            else Assert.Fail("80 - 20 distribution unexpected result");
        }

        float ratio = (float)sumA / (sumA + sumB);
        Assert.AreEqual(0.8, ratio, 0.005);

        //Test 20-80 distribution (Testing opposite direction as well to ensure consistency)
        lootDescription.SetDrops(new DropProbabilityPair[] {
            new DropProbabilityPair() { Drop = testDropA, Probability = 0.2f },
            new DropProbabilityPair(){Drop = testDropB, Probability = 0.8f } });
        sumA = 0;
        sumB = 0;

        for (int i = 0; i < 100000; i++)
        {
            var drop = lootDescription.SelectDropRandomly();
            if (drop == testDropA) sumA++;
            else if (drop == testDropB) sumB++;
            else Assert.Fail("80 - 20 distribution unexpected result");
        }

        ratio = (float)sumA / (sumA + sumB);
        Assert.AreEqual(0.2, ratio, 0.005);


        Object.DestroyImmediate(lootDescription);
        Object.DestroyImmediate(testDropA);
        Object.DestroyImmediate(testDropB);
    }
}
