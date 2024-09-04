using fim.twilight;

namespace fim.spike.Nodes
{
	public class FunctionNode : Node
	{
		public string Name = "";
		public bool IsMain = false;
		public StatementsNode? Body = null;

		public VarType? ReturnType = null;
		// public Dictionary<string, VarType>? ParametersType = null;
		public List<FunctionNodeParameter>? ParametersType = null;

		public struct FunctionNodeParameter
		{
			public string Name;
			public VarType Type;
		}

		public static FunctionNode Parse(AST ast)
		{
			FunctionNode node = new();

			Token? startToken = null;
			if (ast.Check(TokenType.FUNCTION_MAIN))
			{
				node.IsMain = true;
				startToken = ast.Consume(TokenType.FUNCTION_MAIN, "Expected FUNCTION_MAIN");
			}
			Token headerToken = ast.Consume(TokenType.FUNCTION_HEADER, "Expected FUNCTION_HEADER");

			Token nameToken = ast.Consume(TokenType.LITERAL, "Expected LITERAL");
			node.Name = nameToken.Value;

			while (true)
			{

				if (ast.Check(TokenType.FUNCTION_PARAMETER))
				{
					ast.Consume(TokenType.FUNCTION_PARAMETER, "Expected FUNCTION_PARAMETER");
					node.ParametersType = new();

					// var paramTokens = ast.ConsumeUntilMatch(t => t.Type == TokenType.FUNCTION_RETURN || t.Type == TokenType.PUNCTUATION, "Cannot find end of FUNCTION_PARAMETER");
					while(true)
					{
						if (ast.Peek().Type == TokenType.FUNCTION_RETURN) break;
						if (ast.Peek().Type == TokenType.PUNCTUATION)
						{
							if (ast.Peek().Value == ",")
							{
								ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION");
                                continue;
							} 
                            break;
						} 

						Token paramTypeToken = ast.Peek();
						var paramType = Utilities.ConvertTypeHint(paramTypeToken.Type);
						if (paramType == VarType.UNKNOWN) { ast.ThrowSyntaxError(paramTypeToken, "Expected variable type"); }
						ast.Next();

						string literalIdentifier = ast.Consume(TokenType.LITERAL, "Expected LITERAL").Value;

						if (node.ParametersType.Any(param => param.Name == literalIdentifier)) ast.ThrowSyntaxError(paramTypeToken, "Variable already exists");
						node.ParametersType.Add( new FunctionNodeParameter() { Name = literalIdentifier, Type = paramType } );
					}
				}
				else if (ast.Check(TokenType.FUNCTION_RETURN))
				{
					ast.Consume(TokenType.FUNCTION_RETURN, "Expected FUNCTION_RETURN");
					Token typeToken = ast.Peek();
					node.ReturnType = Utilities.ConvertTypeHint(typeToken.Type);
					if (node.ReturnType == VarType.UNKNOWN) { ast.ThrowSyntaxError(typeToken, "Expected variable type"); }
					ast.Next();
				}
				else
				{
					break;
				}
			}

			ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION");

			node.Body = StatementsNode.ParseStatements(ast, TokenType.FUNCTION_FOOTER);

			ast.Consume(TokenType.FUNCTION_FOOTER, "Expected FUNCTION_FOOTER");
			string expectedName = ast.Consume(TokenType.LITERAL, "Expected LITERAL").Value;
			if (node.Name != expectedName) ast.ThrowSyntaxError(nameToken, $"Mismatch method name -> {node.Name} & {expectedName}");
			Token endToken = ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION.");

			node.Start = startToken != null ? startToken.Start : headerToken.Start;
			node.Length = endToken.Start + endToken.Length - node.Start;

			return node;
		}
	}

	public class FunctionCallNode : Node
	{
		public string Identifier = "";
		public List<ValueNode>? Parameters = null;

		public static FunctionCallNode Parse(AST ast)
		{
			FunctionCallNode node = new();

			Token startToken = ast.Consume(TokenType.FUNCTION_CALL, "Expected FUNCTION_CALL");

			node.Identifier = ast.Consume(TokenType.LITERAL, "Expected LITERAL").Value;

			if( ast.Peek().Type == TokenType.FUNCTION_PARAMETER )
			{
				node.Parameters = new();
				ast.Consume(TokenType.FUNCTION_PARAMETER, "Expected FUNCTION_PARAMETER");

                List<Token> paramTokens = ast.ConsumeUntilMatch((p) => p.Type == TokenType.PUNCTUATION && p.Value != ",", "Could not find PUNCTUATION");
				List<Token> tokenBuffer = new();

				foreach(Token t in paramTokens)
				{
					if(t.Type == TokenType.PUNCTUATION && t.Value == "," && tokenBuffer.Count > 0)
					{
						node.Parameters.Add(AST.CreateValueNode(tokenBuffer));
						tokenBuffer.Clear();
						continue;
					}

					tokenBuffer.Add(t);
				}

				if( tokenBuffer.Count > 0 )
                {
                    node.Parameters.Add(AST.CreateValueNode(tokenBuffer));
                    tokenBuffer.Clear();
                }
            }

            Token endToken = ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION.");

			node.Start = startToken.Start;
			node.Length = endToken.Start + endToken.Length - node.Start;

			return node;
		}
	}
}
