using fim.celestia;
using fim.spike;
using fim.spike.Nodes;
using fim.twilight;

namespace fim.tests
{
	[TestClass]
	public class Functions 
	{
		[TestMethod]
		public void Functions_Function()
		{
			string letter = """
				I learned how to create a test.
				That's all about how to create a test.
				""";
			var tokens = Lexer.Parse(letter);

			var report = FunctionNode.Parse(new AST(tokens, letter));

			Assert.AreEqual(report.Name, "how to create a test");
			Assert.IsFalse(report.IsMain);
		}

		[TestMethod]
		public void Functions_MainFunction()
		{
			string letter = """
				Today I learned how to say hello world.
				That's all about how to say hello world.
				""";
			var tokens = Lexer.Parse(letter);

			var report = FunctionNode.Parse(new AST(tokens, letter));

			Assert.AreEqual(report.Name, "how to say hello world");
			Assert.IsTrue(report.IsMain);
		}

		[TestMethod]
		public void Functions_InvalidFunctionName_Throws()
		{
			string letter = """
				I learned how to create a letter.
				That's all about how to create a letter.
				""";
			var tokens = Lexer.Parse(letter);

			Assert.ThrowsException<FiMException>(() => FunctionNode.Parse(new AST(tokens, letter)));
		}

		[TestMethod]
		public void Functions_MissingEnd_Throws()
		{
			string letter = """
				I learned how to write a method.
				""";
			var tokens = Lexer.Parse(letter);

			Assert.ThrowsException<FiMException>(() => FunctionNode.Parse(new AST(tokens, letter)));
		}

		[TestMethod]
		public void Functions_ParamFunction()
		{
			string letter = """
				Dear Princess Celestia: Parameter Function!
				I learned how to accept a parameter using the word Spike.
					I said Spike.
				That's all about how to accept a parameter.

				Today I learned how to run a function.
					I remembered how to accept a parameter using "Hello World".
				That's all about how to run a function.
				Your faithful student, Twilight Sparkle.
				""";
			var tokens = Lexer.Parse(letter);

			var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
			var i = new Interpreter(report, letter);

			var f = i.Paragraphs.FirstOrDefault(p => p.Name == "how to accept a parameter");
			Assert.IsNotNull(f);
			Assert.IsNotNull(f.Parameters);
            Assert.IsTrue(f.Parameters.Any(param => param.Name == "Spike" && param.Type == VarType.STRING));

            using StringWriter sw = new();
            Console.SetOut(sw);
            i.MainParagraph!.Execute();
            Assert.AreEqual("Hello World\n", sw.ToString());
		}

		[TestMethod]
		public void Functions_MultipleParamFunction()
		{
			string letter = """
				Dear Princess Celestia: Multiple Parameter Function!
				I learned how to accept parameters using the number Spike, the number Owlowiscious.
					I said Spike.
					I said Owlowiscious.
				That's all about how to accept parameters.

				Today I learned how to run a function.
					I remembered how to accept parameters using 1, 2.
				That's all about how to run a function.
				Your faithful student, Twilight Sparkle.
				""";
			var tokens = Lexer.Parse(letter);

			var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
			var i = new Interpreter(report, letter);

			var f = i.Paragraphs.FirstOrDefault(p => p.Name == "how to accept parameters");
			Assert.IsNotNull(f);
			Assert.IsNotNull(f.Parameters);
            Assert.IsTrue(f.Parameters.Any(param => param.Name == "Spike" && param.Type == VarType.NUMBER));
            Assert.IsTrue(f.Parameters.Any(param => param.Name == "Owlowiscious" && param.Type == VarType.NUMBER));

            using StringWriter sw = new();
            Console.SetOut(sw);
            i.MainParagraph!.Execute();
            Assert.AreEqual("1\n2\n", sw.ToString());
		}

		[TestMethod]
		public void Functions_MultipleParamDefaultValueFunction()
		{
			string letter = """
				Dear Princess Celestia: Multiple Parameter Function!
				I learned how to accept parameters using the number Spike, the number Owlowiscious.
					I said Spike.
					I said Owlowiscious.
				That's all about how to accept parameters.

				Today I learned how to run a function.
					I remembered how to accept parameters using 1.
				That's all about how to run a function.
				Your faithful student, Twilight Sparkle.
				""";
			var tokens = Lexer.Parse(letter);

			var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
			var i = new Interpreter(report, letter);

			var f = i.Paragraphs.FirstOrDefault(p => p.Name == "how to accept parameters");
			Assert.IsNotNull(f);
			Assert.IsNotNull(f.Parameters);
            Assert.IsTrue(f.Parameters.Any(param => param.Name == "Spike" && param.Type == VarType.NUMBER));
            Assert.IsTrue(f.Parameters.Any(param => param.Name == "Owlowiscious" && param.Type == VarType.NUMBER));

            using StringWriter sw = new();
            Console.SetOut(sw);
            i.MainParagraph!.Execute();
            Assert.AreEqual("1\n0\n", sw.ToString());
		}

		[TestMethod]
		public void Functions_ReturnFunction()
		{
			string letter = """
				Dear Princess Celestia: Return Function!

				I learned how to send a variable to get a word!
					Then you get "Hello!".
				That's all about how to send a variable.

				Today I learned how to run a function.
					I said how to send a variable.
				That's all about how to run a function.
				Your faithful student, Twilight Sparkle.
				""";
			var tokens = Lexer.Parse(letter);

			var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
			var i = new Interpreter(report, letter);

			var f = i.Paragraphs.FirstOrDefault(p => p.Name == "how to send a variable");
			Assert.IsNotNull(f);
			Assert.IsNotNull(f.Returns);
            Assert.IsTrue(f.Returns == VarType.STRING);

            using StringWriter sw = new();
            Console.SetOut(sw);
            i.MainParagraph!.Execute();
            Assert.AreEqual("Hello!\n", sw.ToString());
		}
	}
}
