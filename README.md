# Expense-Manager
This is what I made for a school project.
This Expense Manager is a lightweight (around 14MB) and efficient desktop application designed to help you track your income and expenses. It's built to be resource-friendly.

![License](https://img.shields.io/badge/License-MIT-blue.svg)
[![GitHub top language](https://img.shields.io/github/languages/top/KlausJackson/Expense-Manager?logo=github)](https://github.com/KlausJackson/Expense-Manager)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/KlausJackson/Expense-Manager?logo=github)](https://github.com/KlausJackson/Expense-Manager)
[![GitHub issues](https://img.shields.io/github/issues/KlausJackson/Expense-Manager?logo=github)](https://github.com/KlausJackson/Expense-Manager)
[![GitHub issues](https://img.shields.io/github/issues-closed/KlausJackson/Expense-Manager?logo=github)](https://github.com/KlausJackson/Expense-Manager)
[![GitHub issues](https://img.shields.io/github/issues-pr/KlausJackson/Expense-Manager?logo=github)](https://github.com/KlausJackson/Expense-Manager)
[![GitHub issues](https://img.shields.io/github/issues-pr-closed/KlausJackson/Expense-Manager?logo=github)](https://github.com/KlausJackson/Expense-Manager)
![GitHub last commit](https://img.shields.io/github/last-commit/KlausJackson/Expense-Manager?style=plastic)

![Forks](https://img.shields.io/github/forks/KlausJackson/Expense-Manager.svg)
![Stars](https://img.shields.io/github/stars/KlausJackson/Expense-Manager.svg)
![Watchers](https://img.shields.io/github/watchers/KlausJackson/Expense-Manager.svg)

This repo first commit: [Mar 13, 2025](https://github.com/KlausJackson/Expense-Manager/commits/main?after=2637ba4f72031e8af6516213d424f0ad5ac2f55d+69)

## Preview

![Demo](Preview.gif)

## How To Contact Me

[![Patreon](https://img.shields.io/badge/Patreon-AC7AC2?style=for-the-badge&logo=patreon&logoColor=white)](patreon.com/KlausJackson)
[![Buy Me A Coffee](https://img.shields.io/badge/BuyCoffee-FFFF00?style=for-the-badge&logo=buymeacoffee&logoColor=black)](https://buymeacoffee.com/KlausJackson)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/KlausJackson/)
[![Gmail](https://img.shields.io/badge/Gmail-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:KlausJackson2@gmail.com)
[![Steam](https://img.shields.io/badge/Steam-000050?style=for-the-badge&logo=steam&logoColor=white)](https://steamcommunity.com/id/KlausJackson/)
[![Facebook](https://img.shields.io/badge/Facebook-0000FF?style=for-the-badge&logo=facebook&logoColor=white)](https://facebook.com/KlausJacksonV)

<hr>

## Requirements

### Packages

This project requires the following NuGet packages. NuGet Package Restore should automatically download and install these when you open the solution:

| Package Name | Version | Description |
| ------------ | ------- | ----------- |
| BouncyCastle.Cryptography | 2.5.1 | Provides cryptographic algorithms. |
| EntityFramework | 6.5.1 | An object-relational mapper (ORM) for .NET Framework. |
| Enums.NET | 5.0.0 | Provides efficient type-safe .NET enums. |
| ExtendedNumerics.BigDecimal | 2025.1001.2.129 | Provides BigDecimal data type for large real numbers. |
| MathNet.Numerics.Signed | 5.0.0 | Aims to provide methods and algorithms for numerical computations in .Net under a consistent framework, while at the same time covering a wide range of topics. |
| Microsoft.Bcl.AsyncInterfaces | 9.0.3 | Provides interfaces for asynchronous operations. |
| Microsoft.Extensions.DependencyInjection | 9.0.3 | Provides support for dependency injection. |
| Microsoft.Extensions.DependencyInjection.Abstractions | 9.0.3 | Provides abstractions for dependency injection. |
| Microsoft.Extensions.Logging | 9.0.3 | Provides logging APIs. |
| Microsoft.Extensions.Logging.Abstractions | 9.0.3 | Provides abstractions for logging APIs. |
| Microsoft.Extensions.Options | 9.0.3 | Provides support for options pattern. |
| Microsoft.Extensions.Primitives | 9.0.3 | Provides core types for building .NET libraries. |
| Microsoft.IO.RecyclableMemoryStream | 3.0.0 | Provides a reusable memory stream implementation. |
| Newtonsoft.Json | 13.0.3 | A popular JSON serialization and deserialization library. |
| NPOI | 2.7.2 | A library for reading and writing Microsoft Office file formats (especially Excel). |
| SharpZipLib | 1.4.2 | A library for ZIP compression and decompression. |
| SixLabors.Fonts | 1.0.1 | A cross-platform font library for image processing. |
| SixLabors.ImageSharp | 3.1.7 | A cross-platform image processing library. |
| System.Buffers | 4.6.0 | Provides types for working with memory buffers. |
| System.Diagnostics.DiagnosticSource | 9.0.3 | Provides APIs for tracing and profiling applications. |
| System.IO | 4.3.0 | Provides types for file I/O operations. |
| System.Memory  | 4.5.5 | Provides types for working with memory regions. |
| System.Numerics.Vectors | 4.5.0 | Provides types for working with SIMD vectors. |
| System.Runtime | 4.3.0 | Provides core types for the .NET runtime. |
| System.Runtime.CompilerServices.Unsafe | 6.1.0 | Provides attributes that are used by language compilers to emit certain kinds of code. |
| System.Security.Cryptography.Algorithms | 4.3.1 | Provides base classes for cryptographic algorithms. |
| System.Security.Cryptography.Encoding | 4.3.0 | Provides types for encoding cryptographic data. |
| System.Security.Cryptography.Pkcs | 8.0.1 | Provides support for the PKCS#7 cryptographic message syntax. |
| System.Security.Cryptography.Primitives | 4.3.0 | Provides base classes for cryptographic primitives. |
| System.Security.Cryptography.Xml | 8.0.2 | Provides classes for manipulating XML documents with cryptographic signatures. |
| System.Text.Encoding.CodePages | 9.0.3 | Provides code pages for character encodings. |
| System.Threading.Tasks.Extensions | 4.6.0 | Provides types that simplify the consumption of Task-based asynchronous patterns. |
| System.ValueTuple | 4.5.0 | Provides value tuple types. |

### SQL Server Set Up

1.  Run the SQL queries in `SQLQuery1.sql` to create the database schema.
2.  Execute the last 3 lines of the script to retrieve the connection string.
```sql
SELECT 
    'Data Source=' + @@SERVERNAME + 
    ';Initial Catalog=' + DB_NAME() + 
    ';Integrated Security=True;' AS ConnectionString
```
3.  Replace the placeholder `connectionString` in `login.cs` with the retrieved connection string.

## Features

**User Authentication:** Securely manage your financial data with user accounts.
**Data Input:**
- Record expenses with details like date, description, amount, and category.
- Record income with date, amount, and description.

**Categorization:**
- Create and manage expense categories (e.g., food, utilities, rent).
- Set a monthly spending goal for each category.

**Data Import/Export:** Import or export financial data from CSV, JSON, and Excel files.

**Search Functionality:** Quickly find specific transactions using various criteria.

**Statistical Data Visualization:**
- **Expense Distribution by Category:** Pie chart showing the proportion of expenses in each category.
- **Category Expenses vs. Monthly Spend:** Column chart comparing actual expenses in each category to the set monthly spending goal.
- **Monthly Income Trend:** Line chart displaying the income trend over time.
- **Monthly Expense Trend:** Line chart displaying the expense trend over time.
- **Monthly Income vs. Expenses:** Chart comparing total monthly income against total monthly expenses.

## The End

If you have any problem while running the code or any ideas to improve it, feel free to file an issue.
I'll probably make a Tauri version for this.

That's all I got. Enjoy!