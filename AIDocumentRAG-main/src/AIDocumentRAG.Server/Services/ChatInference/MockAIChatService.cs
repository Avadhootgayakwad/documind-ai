namespace AIDocumentRAG.Server.Services.ChatInference
{
    using AIDocumentRAG.Server.Core.ChatInference;

    /// <summary>
    /// Mock AI Chat Service for testing without external AI providers.
    /// Provides basic responses based on simple pattern matching.
    /// </summary>
    public class MockAIChatService : IAIChatService
    {
        private readonly ILogger<MockAIChatService> _logger;
        private int _responseCounter = 0;

        public MockAIChatService(ILogger<MockAIChatService> logger)
        {
            _logger = logger;
        }

        public Task<string> GenerateResponseAsync(string prompt, string servicer, string model)
        {
            _logger.LogInformation("Mock AI processing prompt: {Prompt}", prompt.Substring(0, Math.Min(100, prompt.Length)));

            string response = GenerateMockResponse(prompt);
            
            return Task.FromResult(response);
        }

        public async IAsyncEnumerable<string> GenerateResponseStreamAsync(string prompt, string servicer, string model)
        {
            _logger.LogInformation("Mock AI streaming response for prompt: {Prompt}", prompt.Substring(0, Math.Min(100, prompt.Length)));

            string response = GenerateMockResponse(prompt);
            
            // Simulate streaming by yielding chunks
            string[] chunks = response.Split(' ');
            foreach (string chunk in chunks)
            {
                yield return chunk + " ";
                await Task.Delay(50); // Small delay to simulate streaming
            }
        }

        private string GenerateMockResponse(string prompt)
        {
            _responseCounter++;
            string lowerPrompt = prompt.ToLower();
            int variation = _responseCounter % 3;

            // Simple pattern matching for common requests with variations
            if (lowerPrompt.Contains("summarize") || lowerPrompt.Contains("summary"))
            {
                string[] summaries = new string[]
                {
                    @"# Document Summary

Based on the document content, here are the key points:

## Main Topics
- The document covers important concepts and information
- Key themes are presented throughout the content
- Various aspects are discussed in detail

## Key Takeaways
1. The document provides comprehensive coverage of the subject matter
2. Important details and examples are included
3. The content is structured to facilitate understanding

## Conclusion
This document contains valuable information that would be useful for understanding the topic in depth.",
                    @"# Summary of Document Content

Here's what the document covers:

## Core Concepts
- Primary topics and themes are thoroughly explored
- Supporting evidence and examples are provided
- The information is organized logically

## Important Points
- The document establishes foundational knowledge
- It builds upon concepts progressively
- Practical applications are discussed

## Final Thoughts
The document serves as a comprehensive resource for understanding the subject matter and provides actionable insights.",
                    @"# Document Overview

## Summary
This document provides detailed information on the topic with several key highlights:

## Major Themes
1. **Primary Focus**: The main subject is explored in depth
2. **Supporting Details**: Evidence and examples reinforce key points
3. **Practical Applications**: Real-world implications are discussed

## Key Insights
- The content is well-structured and comprehensive
- Important concepts are clearly explained
- The document serves as a valuable reference material

Would you like me to elaborate on any specific section?"
                };
                return summaries[variation];
            }

            if (lowerPrompt.Contains("what is") || lowerPrompt.Contains("explain"))
            {
                string[] explanations = new string[]
                {
                    @"# Explanation

Based on the document provided, I can explain this concept:

## Overview
The document discusses this topic in detail, covering various aspects and providing context.

## Key Points
- The concept is explained with relevant examples
- Important details are highlighted throughout the document
- The information is presented in a structured manner

## Additional Context
The document provides background information that helps understand this topic better.",
                    @"# Detailed Explanation

## Understanding the Concept
According to the document, this topic encompasses several important elements:

## Main Components
1. **Foundation**: The basic principles are established early
2. **Development**: The concept is expanded with examples
3. **Application**: Practical uses are demonstrated

## Key Insights
The document breaks down complex ideas into understandable segments. Each section builds upon previous knowledge to create a comprehensive understanding.",
                    @"# Concept Explanation

## What the Document Says
The provided document offers a thorough examination of this topic:

## Detailed Breakdown
- **Definition**: The concept is clearly defined in the text
- **Context**: It's placed within a broader framework
- **Examples**: Real-world applications are provided
- **Implications**: The significance is discussed

The document makes this topic accessible through clear explanations and relevant examples throughout the content."
                };
                return explanations[variation];
            }

            if (lowerPrompt.Contains("list") || lowerPrompt.Contains("what are"))
            {
                string[] lists = new string[]
                {
                    @"# Items from the Document

Based on the document content, here are the relevant items:

1. **First Item** - Described in the document with relevant details
2. **Second Item** - Explained with examples and context
3. **Third Item** - Covered with additional information
4. **Fourth Item** - Discussed in relation to other topics
5. **Fifth Item** - Presented with supporting evidence

These items are extracted from the document content.",
                    @"# Document Items

Here's what the document lists:

## Key Elements
1. **Element One**: Introduction and basic overview
2. **Element Two**: Detailed analysis and discussion
3. **Element Three**: Practical implementation strategies
4. **Element Four**: Case studies and examples
5. **Element Five**: Future considerations and implications

Each element is thoroughly covered in its respective section of the document.",
                    @"# Comprehensive List

Based on my analysis of the document, here are the main items:

## Extracted Items
- **Item 1**: Core concept with foundational importance
- **Item 2**: Advanced topic building on basics
- **Item 3**: Practical application methodology
- **Item 4**: Evaluation and assessment criteria
- **Item 5**: Best practices and recommendations

The document provides detailed information for each of these items in separate sections."
                };
                return lists[variation];
            }

            if (lowerPrompt.Contains("compare") || lowerPrompt.Contains("difference"))
            {
                string[] comparisons = new string[]
                {
                    @"# Comparison

Based on the document, here's a comparison:

## Similarities
- Both concepts share common foundations
- They are related in several important ways
- The document discusses their connections

## Differences
- **Aspect 1**: Each has distinct characteristics
- **Aspect 2**: Different approaches are presented
- **Aspect 3**: Various implications are discussed

The document provides detailed analysis of both similarities and differences.",
                    @"# Comparative Analysis

## Common Ground
The document reveals several similarities:
- Shared underlying principles
- Complementary approaches
- Overlapping objectives

## Key Distinctions
1. **Methodology**: Different approaches to achieving goals
2. **Implementation**: Varied practical applications
3. **Outcomes**: Distinct results and benefits

The document presents a balanced view of both similarities and differences, allowing for informed decision-making.",
                    @"# Comparison Overview

## What's Similar
According to the document, these concepts share:
- Foundational principles and理论基础
- Common objectives and goals
- Related implementation strategies

## What's Different
- **Approach 1 vs Approach 2**: Different methodologies
- **Use Case A vs Use Case B**: Different applications
- **Outcome X vs Outcome Y**: Different results

The document thoroughly examines both perspectives and provides guidance on when to use each approach."
                };
                return comparisons[variation];
            }

            // Default response with more variety
            string[] defaultResponses = new string[]
            {
                $@"# Response to Your Question

Thank you for your question about: **{prompt.Substring(0, Math.Min(80, prompt.Length))}**

## Analysis
Based on the document content provided, I can offer the following insights:

The document contains relevant information that addresses your query. The content covers various aspects of the topic and provides useful context.

## Key Observations
- The document discusses related concepts and information
- Important details are presented throughout the text
- The content provides a foundation for understanding this topic

## Recommendation
For the most accurate and detailed answer, I recommend reviewing the specific sections of the document related to your question.

Would you like me to elaborate on any specific aspect?",
                $@"# Answer to Your Query

Your question about **{prompt.Substring(0, Math.Min(80, prompt.Length))}** is addressed in the document:

## Document Insights
The provided content offers valuable information on this topic:

1. **Context**: The document establishes the necessary background
2. **Details**: Specific information relevant to your question is included
3. **Examples**: Practical demonstrations are provided

## Important Notes
- The document approaches this topic from multiple angles
- Key concepts are explained with supporting evidence
- The information is organized for easy reference

Feel free to ask follow-up questions if you need clarification on any point!",
                $@"# My Response

Regarding your question: **{prompt.Substring(0, Math.Min(80, prompt.Length))}**

## What the Document Tells Us
After analyzing the document content, here are my findings:

### Key Points
- The topic is covered in relevant sections
- Important details support the main concepts
- The document provides both theory and practical information

### Additional Context
The document's treatment of this subject is comprehensive, covering:
- Foundational concepts
- Advanced applications
- Real-world examples

I'm here to help you understand any specific aspects in more detail!"
            };
            return defaultResponses[variation];
        }
    }
}
