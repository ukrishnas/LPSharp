/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 4.0.2
 *
 * This file is not intended to be easily readable and contains a number of
 * coding conventions designed to improve portability and efficiency. Do not make
 * changes to this file unless you know what you are doing--modify the SWIG
 * interface file instead.
 * ----------------------------------------------------------------------------- */


#ifndef SWIGCSHARP
#define SWIGCSHARP
#endif



#ifdef __cplusplus
/* SwigValueWrapper is described in swig.swg */
template<typename T> class SwigValueWrapper {
  struct SwigMovePointer {
    T *ptr;
    SwigMovePointer(T *p) : ptr(p) { }
    ~SwigMovePointer() { delete ptr; }
    SwigMovePointer& operator=(SwigMovePointer& rhs) { T* oldptr = ptr; ptr = 0; delete oldptr; ptr = rhs.ptr; rhs.ptr = 0; return *this; }
  } pointer;
  SwigValueWrapper& operator=(const SwigValueWrapper<T>& rhs);
  SwigValueWrapper(const SwigValueWrapper<T>& rhs);
public:
  SwigValueWrapper() : pointer(0) { }
  SwigValueWrapper& operator=(const T& t) { SwigMovePointer tmp(new T(t)); pointer = tmp; return *this; }
  operator T&() const { return *pointer.ptr; }
  T *operator&() { return pointer.ptr; }
};

template <typename T> T SwigValueInit() {
  return T();
}
#endif

/* -----------------------------------------------------------------------------
 *  This section contains generic SWIG labels for method/variable
 *  declarations/attributes, and other compiler dependent labels.
 * ----------------------------------------------------------------------------- */

/* template workaround for compilers that cannot correctly implement the C++ standard */
#ifndef SWIGTEMPLATEDISAMBIGUATOR
# if defined(__SUNPRO_CC) && (__SUNPRO_CC <= 0x560)
#  define SWIGTEMPLATEDISAMBIGUATOR template
# elif defined(__HP_aCC)
/* Needed even with `aCC -AA' when `aCC -V' reports HP ANSI C++ B3910B A.03.55 */
/* If we find a maximum version that requires this, the test would be __HP_aCC <= 35500 for A.03.55 */
#  define SWIGTEMPLATEDISAMBIGUATOR template
# else
#  define SWIGTEMPLATEDISAMBIGUATOR
# endif
#endif

/* inline attribute */
#ifndef SWIGINLINE
# if defined(__cplusplus) || (defined(__GNUC__) && !defined(__STRICT_ANSI__))
#   define SWIGINLINE inline
# else
#   define SWIGINLINE
# endif
#endif

/* attribute recognised by some compilers to avoid 'unused' warnings */
#ifndef SWIGUNUSED
# if defined(__GNUC__)
#   if !(defined(__cplusplus)) || (__GNUC__ > 3 || (__GNUC__ == 3 && __GNUC_MINOR__ >= 4))
#     define SWIGUNUSED __attribute__ ((__unused__))
#   else
#     define SWIGUNUSED
#   endif
# elif defined(__ICC)
#   define SWIGUNUSED __attribute__ ((__unused__))
# else
#   define SWIGUNUSED
# endif
#endif

#ifndef SWIG_MSC_UNSUPPRESS_4505
# if defined(_MSC_VER)
#   pragma warning(disable : 4505) /* unreferenced local function has been removed */
# endif
#endif

#ifndef SWIGUNUSEDPARM
# ifdef __cplusplus
#   define SWIGUNUSEDPARM(p)
# else
#   define SWIGUNUSEDPARM(p) p SWIGUNUSED
# endif
#endif

/* internal SWIG method */
#ifndef SWIGINTERN
# define SWIGINTERN static SWIGUNUSED
#endif

/* internal inline SWIG method */
#ifndef SWIGINTERNINLINE
# define SWIGINTERNINLINE SWIGINTERN SWIGINLINE
#endif

/* exporting methods */
#if defined(__GNUC__)
#  if (__GNUC__ >= 4) || (__GNUC__ == 3 && __GNUC_MINOR__ >= 4)
#    ifndef GCC_HASCLASSVISIBILITY
#      define GCC_HASCLASSVISIBILITY
#    endif
#  endif
#endif

#ifndef SWIGEXPORT
# if defined(_WIN32) || defined(__WIN32__) || defined(__CYGWIN__)
#   if defined(STATIC_LINKED)
#     define SWIGEXPORT
#   else
#     define SWIGEXPORT __declspec(dllexport)
#   endif
# else
#   if defined(__GNUC__) && defined(GCC_HASCLASSVISIBILITY)
#     define SWIGEXPORT __attribute__ ((visibility("default")))
#   else
#     define SWIGEXPORT
#   endif
# endif
#endif

/* calling conventions for Windows */
#ifndef SWIGSTDCALL
# if defined(_WIN32) || defined(__WIN32__) || defined(__CYGWIN__)
#   define SWIGSTDCALL __stdcall
# else
#   define SWIGSTDCALL
# endif
#endif

/* Deal with Microsoft's attempt at deprecating C standard runtime functions */
#if !defined(SWIG_NO_CRT_SECURE_NO_DEPRECATE) && defined(_MSC_VER) && !defined(_CRT_SECURE_NO_DEPRECATE)
# define _CRT_SECURE_NO_DEPRECATE
#endif

/* Deal with Microsoft's attempt at deprecating methods in the standard C++ library */
#if !defined(SWIG_NO_SCL_SECURE_NO_DEPRECATE) && defined(_MSC_VER) && !defined(_SCL_SECURE_NO_DEPRECATE)
# define _SCL_SECURE_NO_DEPRECATE
#endif

/* Deal with Apple's deprecated 'AssertMacros.h' from Carbon-framework */
#if defined(__APPLE__) && !defined(__ASSERT_MACROS_DEFINE_VERSIONS_WITHOUT_UNDERSCORES)
# define __ASSERT_MACROS_DEFINE_VERSIONS_WITHOUT_UNDERSCORES 0
#endif

/* Intel's compiler complains if a variable which was never initialised is
 * cast to void, which is a common idiom which we use to indicate that we
 * are aware a variable isn't used.  So we just silence that warning.
 * See: https://github.com/swig/swig/issues/192 for more discussion.
 */
#ifdef __INTEL_COMPILER
# pragma warning disable 592
#endif


#include <stdlib.h>
#include <string.h>
#include <stdio.h>


/* Support for throwing C# exceptions from C/C++. There are two types: 
 * Exceptions that take a message and ArgumentExceptions that take a message and a parameter name. */
typedef enum {
  SWIG_CSharpApplicationException,
  SWIG_CSharpArithmeticException,
  SWIG_CSharpDivideByZeroException,
  SWIG_CSharpIndexOutOfRangeException,
  SWIG_CSharpInvalidCastException,
  SWIG_CSharpInvalidOperationException,
  SWIG_CSharpIOException,
  SWIG_CSharpNullReferenceException,
  SWIG_CSharpOutOfMemoryException,
  SWIG_CSharpOverflowException,
  SWIG_CSharpSystemException
} SWIG_CSharpExceptionCodes;

typedef enum {
  SWIG_CSharpArgumentException,
  SWIG_CSharpArgumentNullException,
  SWIG_CSharpArgumentOutOfRangeException
} SWIG_CSharpExceptionArgumentCodes;

typedef void (SWIGSTDCALL* SWIG_CSharpExceptionCallback_t)(const char *);
typedef void (SWIGSTDCALL* SWIG_CSharpExceptionArgumentCallback_t)(const char *, const char *);

typedef struct {
  SWIG_CSharpExceptionCodes code;
  SWIG_CSharpExceptionCallback_t callback;
} SWIG_CSharpException_t;

typedef struct {
  SWIG_CSharpExceptionArgumentCodes code;
  SWIG_CSharpExceptionArgumentCallback_t callback;
} SWIG_CSharpExceptionArgument_t;

static SWIG_CSharpException_t SWIG_csharp_exceptions[] = {
  { SWIG_CSharpApplicationException, NULL },
  { SWIG_CSharpArithmeticException, NULL },
  { SWIG_CSharpDivideByZeroException, NULL },
  { SWIG_CSharpIndexOutOfRangeException, NULL },
  { SWIG_CSharpInvalidCastException, NULL },
  { SWIG_CSharpInvalidOperationException, NULL },
  { SWIG_CSharpIOException, NULL },
  { SWIG_CSharpNullReferenceException, NULL },
  { SWIG_CSharpOutOfMemoryException, NULL },
  { SWIG_CSharpOverflowException, NULL },
  { SWIG_CSharpSystemException, NULL }
};

static SWIG_CSharpExceptionArgument_t SWIG_csharp_exceptions_argument[] = {
  { SWIG_CSharpArgumentException, NULL },
  { SWIG_CSharpArgumentNullException, NULL },
  { SWIG_CSharpArgumentOutOfRangeException, NULL }
};

static void SWIGUNUSED SWIG_CSharpSetPendingException(SWIG_CSharpExceptionCodes code, const char *msg) {
  SWIG_CSharpExceptionCallback_t callback = SWIG_csharp_exceptions[SWIG_CSharpApplicationException].callback;
  if ((size_t)code < sizeof(SWIG_csharp_exceptions)/sizeof(SWIG_CSharpException_t)) {
    callback = SWIG_csharp_exceptions[code].callback;
  }
  callback(msg);
}

static void SWIGUNUSED SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpExceptionArgumentCodes code, const char *msg, const char *param_name) {
  SWIG_CSharpExceptionArgumentCallback_t callback = SWIG_csharp_exceptions_argument[SWIG_CSharpArgumentException].callback;
  if ((size_t)code < sizeof(SWIG_csharp_exceptions_argument)/sizeof(SWIG_CSharpExceptionArgument_t)) {
    callback = SWIG_csharp_exceptions_argument[code].callback;
  }
  callback(msg, param_name);
}


#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL SWIGRegisterExceptionCallbacks_coinwrap(
                                                SWIG_CSharpExceptionCallback_t applicationCallback,
                                                SWIG_CSharpExceptionCallback_t arithmeticCallback,
                                                SWIG_CSharpExceptionCallback_t divideByZeroCallback, 
                                                SWIG_CSharpExceptionCallback_t indexOutOfRangeCallback, 
                                                SWIG_CSharpExceptionCallback_t invalidCastCallback,
                                                SWIG_CSharpExceptionCallback_t invalidOperationCallback,
                                                SWIG_CSharpExceptionCallback_t ioCallback,
                                                SWIG_CSharpExceptionCallback_t nullReferenceCallback,
                                                SWIG_CSharpExceptionCallback_t outOfMemoryCallback, 
                                                SWIG_CSharpExceptionCallback_t overflowCallback, 
                                                SWIG_CSharpExceptionCallback_t systemCallback) {
  SWIG_csharp_exceptions[SWIG_CSharpApplicationException].callback = applicationCallback;
  SWIG_csharp_exceptions[SWIG_CSharpArithmeticException].callback = arithmeticCallback;
  SWIG_csharp_exceptions[SWIG_CSharpDivideByZeroException].callback = divideByZeroCallback;
  SWIG_csharp_exceptions[SWIG_CSharpIndexOutOfRangeException].callback = indexOutOfRangeCallback;
  SWIG_csharp_exceptions[SWIG_CSharpInvalidCastException].callback = invalidCastCallback;
  SWIG_csharp_exceptions[SWIG_CSharpInvalidOperationException].callback = invalidOperationCallback;
  SWIG_csharp_exceptions[SWIG_CSharpIOException].callback = ioCallback;
  SWIG_csharp_exceptions[SWIG_CSharpNullReferenceException].callback = nullReferenceCallback;
  SWIG_csharp_exceptions[SWIG_CSharpOutOfMemoryException].callback = outOfMemoryCallback;
  SWIG_csharp_exceptions[SWIG_CSharpOverflowException].callback = overflowCallback;
  SWIG_csharp_exceptions[SWIG_CSharpSystemException].callback = systemCallback;
}

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL SWIGRegisterExceptionArgumentCallbacks_coinwrap(
                                                SWIG_CSharpExceptionArgumentCallback_t argumentCallback,
                                                SWIG_CSharpExceptionArgumentCallback_t argumentNullCallback,
                                                SWIG_CSharpExceptionArgumentCallback_t argumentOutOfRangeCallback) {
  SWIG_csharp_exceptions_argument[SWIG_CSharpArgumentException].callback = argumentCallback;
  SWIG_csharp_exceptions_argument[SWIG_CSharpArgumentNullException].callback = argumentNullCallback;
  SWIG_csharp_exceptions_argument[SWIG_CSharpArgumentOutOfRangeException].callback = argumentOutOfRangeCallback;
}


/* Callback for returning strings to C# without leaking memory */
typedef char * (SWIGSTDCALL* SWIG_CSharpStringHelperCallback)(const char *);
static SWIG_CSharpStringHelperCallback SWIG_csharp_string_callback = NULL;


#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL SWIGRegisterStringCallback_coinwrap(SWIG_CSharpStringHelperCallback callback) {
  SWIG_csharp_string_callback = callback;
}


/* Contract support */

#define SWIG_contract_assert(nullreturn, expr, msg) if (!(expr)) {SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentOutOfRangeException, msg, ""); return nullreturn; } else


#include <stdint.h>		// Use the C99 official header


#ifdef __cplusplus
extern "C" {
#endif

SWIGEXPORT void * SWIGSTDCALL CSharp_CoinOrfClp_new_ClpInterface___() {
  void * jresult ;
  coinwrap::ClpInterface *result = 0 ;
  
  result = (coinwrap::ClpInterface *)new coinwrap::ClpInterface();
  jresult = (void *)result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_delete_ClpInterface___(void * jarg1) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  delete arg1;
}


SWIGEXPORT double SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_OptimizationDirection___(void * jarg1) {
  double jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (double)(arg1)->OptimizationDirection();
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetOptimizationDirection___(void * jarg1, double jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double arg2 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (double)jarg2; 
  (arg1)->SetOptimizationDirection(arg2);
}


SWIGEXPORT double SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_DualTolerance___(void * jarg1) {
  double jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (double)(arg1)->DualTolerance();
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetDualTolerance___(void * jarg1, double jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double arg2 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (double)jarg2; 
  (arg1)->SetDualTolerance(arg2);
}


SWIGEXPORT double SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_PrimalTolerance___(void * jarg1) {
  double jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (double)(arg1)->PrimalTolerance();
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetPrimalTolerance___(void * jarg1, double jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double arg2 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (double)jarg2; 
  (arg1)->SetPrimalTolerance(arg2);
}


SWIGEXPORT double SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_PrimalWeight___(void * jarg1) {
  double jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (double)(arg1)->PrimalWeight();
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetPrimalWeight___(void * jarg1, double jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double arg2 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (double)jarg2; 
  (arg1)->SetPrimalWeight(arg2);
}


SWIGEXPORT double SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_PositiveEdgePsi___(void * jarg1) {
  double jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (double)(arg1)->PositiveEdgePsi();
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetPositiveEdgePsi___(void * jarg1, double jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double arg2 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (double)jarg2; 
  (arg1)->SetPositiveEdgePsi(arg2);
}


SWIGEXPORT int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_Perturbation___(void * jarg1) {
  int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  int result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (int)(arg1)->Perturbation();
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetPerturbation___(void * jarg1, int jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  int arg2 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (int)jarg2; 
  (arg1)->SetPerturbation(arg2);
}


SWIGEXPORT int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_MaximumIterations___(void * jarg1) {
  int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  int result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (int)(arg1)->MaximumIterations();
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetMaximumIterations___(void * jarg1, int jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  int arg2 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (int)jarg2; 
  (arg1)->SetMaximumIterations(arg2);
}


SWIGEXPORT double SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_MaximumSeconds___(void * jarg1) {
  double jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (double)(arg1)->MaximumSeconds();
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetMaximumSeconds___(void * jarg1, double jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double arg2 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (double)jarg2; 
  (arg1)->SetMaximumSeconds(arg2);
}


SWIGEXPORT double SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SolveTimeMs___(void * jarg1) {
  double jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (double)(arg1)->SolveTimeMs();
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetLogLevel___(void * jarg1, int jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  int arg2 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (int)jarg2; 
  (arg1)->SetLogLevel(arg2);
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_Reset___(void * jarg1) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  (arg1)->Reset();
}


SWIGEXPORT unsigned int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_ReadMps___(void * jarg1, char * jarg2) {
  unsigned int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  char *arg2 = (char *) 0 ;
  bool result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (char *)jarg2; 
  result = (bool)(arg1)->ReadMps((char const *)arg2);
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_WriteMps___(void * jarg1, char * jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  char *arg2 = (char *) 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (char *)jarg2; 
  (arg1)->WriteMps((char const *)arg2);
}


SWIGEXPORT unsigned int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetPrimalPivotAlgorithm___(void * jarg1, int jarg2) {
  unsigned int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  coinwrap::PivotAlgorithm arg2 ;
  bool result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (coinwrap::PivotAlgorithm)jarg2; 
  result = (bool)(arg1)->SetPrimalPivotAlgorithm(arg2);
  jresult = result; 
  return jresult;
}


SWIGEXPORT unsigned int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetDualPivotAlgorithm___(void * jarg1, int jarg2) {
  unsigned int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  coinwrap::PivotAlgorithm arg2 ;
  bool result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (coinwrap::PivotAlgorithm)jarg2; 
  result = (bool)(arg1)->SetDualPivotAlgorithm(arg2);
  jresult = result; 
  return jresult;
}


SWIGEXPORT int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_PresolvePasses___(void * jarg1) {
  int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  int result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (int)(arg1)->PresolvePasses();
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetPresolvePasses___(void * jarg1, int jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  int arg2 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (int)jarg2; 
  (arg1)->SetPresolvePasses(arg2);
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_MakePlusMinusOneMatrix___(void * jarg1, unsigned int jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  bool arg2 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = jarg2 ? true : false; 
  (arg1)->MakePlusMinusOneMatrix(arg2);
}


SWIGEXPORT int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_DualStartingBasis___(void * jarg1) {
  int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  coinwrap::StartingBasis result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (coinwrap::StartingBasis)(arg1)->DualStartingBasis();
  jresult = (int)result; 
  return jresult;
}


SWIGEXPORT unsigned int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetDualStartingBasis___(void * jarg1, int jarg2) {
  unsigned int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  coinwrap::StartingBasis arg2 ;
  bool result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (coinwrap::StartingBasis)jarg2; 
  result = (bool)(arg1)->SetDualStartingBasis(arg2);
  jresult = result; 
  return jresult;
}


SWIGEXPORT int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_PrimalStartingBasis___(void * jarg1) {
  int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  coinwrap::StartingBasis result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (coinwrap::StartingBasis)(arg1)->PrimalStartingBasis();
  jresult = (int)result; 
  return jresult;
}


SWIGEXPORT unsigned int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetPrimalStartingBasis___(void * jarg1, int jarg2) {
  unsigned int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  coinwrap::StartingBasis arg2 ;
  bool result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (coinwrap::StartingBasis)jarg2; 
  result = (bool)(arg1)->SetPrimalStartingBasis(arg2);
  jresult = result; 
  return jresult;
}


SWIGEXPORT int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_GetSolveType___(void * jarg1) {
  int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  coinwrap::SolveType result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (coinwrap::SolveType)(arg1)->GetSolveType();
  jresult = (int)result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetSolveType___(void * jarg1, int jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  coinwrap::SolveType arg2 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (coinwrap::SolveType)jarg2; 
  (arg1)->SetSolveType(arg2);
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_Solve___(void * jarg1) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  (arg1)->Solve();
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SolveUsingDualSimplex___(void * jarg1) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  (arg1)->SolveUsingDualSimplex();
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SolveUsingDualCrash___(void * jarg1) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  (arg1)->SolveUsingDualCrash();
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SolveUsingPrimalSimplex___(void * jarg1) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  (arg1)->SolveUsingPrimalSimplex();
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SolveUsingPrimalIdiot___(void * jarg1) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  (arg1)->SolveUsingPrimalIdiot();
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SolveUsingEitherSimplex___(void * jarg1) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  (arg1)->SolveUsingEitherSimplex();
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SolveUsingBarrierMethod___(void * jarg1) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  (arg1)->SolveUsingBarrierMethod();
}


SWIGEXPORT double SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_ObjectiveValue___(void * jarg1) {
  double jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  double result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (double)(arg1)->ObjectiveValue();
  jresult = result; 
  return jresult;
}


SWIGEXPORT int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_Iterations___(void * jarg1) {
  int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  int result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (int)(arg1)->Iterations();
  jresult = result; 
  return jresult;
}


SWIGEXPORT int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_Status___(void * jarg1) {
  int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  coinwrap::ClpStatus result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (coinwrap::ClpStatus)(arg1)->Status();
  jresult = (int)result; 
  return jresult;
}


SWIGEXPORT int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SecondaryStatus___(void * jarg1) {
  int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  int result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  result = (int)(arg1)->SecondaryStatus();
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_PrimalColumnSolution___(void * jarg1, void * jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  std::vector< double > *arg2 = 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (std::vector< double > *)jarg2;
  if (!arg2) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "std::vector< double > & type is null", 0);
    return ;
  } 
  (arg1)->PrimalColumnSolution(*arg2);
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_DualColumnSolution___(void * jarg1, void * jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  std::vector< double > *arg2 = 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (std::vector< double > *)jarg2;
  if (!arg2) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "std::vector< double > & type is null", 0);
    return ;
  } 
  (arg1)->DualColumnSolution(*arg2);
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_PrimalRowSolution___(void * jarg1, void * jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  std::vector< double > *arg2 = 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (std::vector< double > *)jarg2;
  if (!arg2) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "std::vector< double > & type is null", 0);
    return ;
  } 
  (arg1)->PrimalRowSolution(*arg2);
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_DualRowSolution___(void * jarg1, void * jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  std::vector< double > *arg2 = 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (std::vector< double > *)jarg2;
  if (!arg2) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "std::vector< double > & type is null", 0);
    return ;
  } 
  (arg1)->DualRowSolution(*arg2);
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_Objective___(void * jarg1, void * jarg2) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  std::vector< double > *arg2 = 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (std::vector< double > *)jarg2;
  if (!arg2) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "std::vector< double > & type is null", 0);
    return ;
  } 
  (arg1)->Objective(*arg2);
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_StartModel___(void * jarg1) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  (arg1)->StartModel();
}


SWIGEXPORT int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_AddVariable___(void * jarg1, char * jarg2, double jarg3, double jarg4) {
  int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  char *arg2 = (char *) 0 ;
  double arg3 ;
  double arg4 ;
  int result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (char *)jarg2; 
  arg3 = (double)jarg3; 
  arg4 = (double)jarg4; 
  result = (int)(arg1)->AddVariable((char const *)arg2,arg3,arg4);
  jresult = result; 
  return jresult;
}


SWIGEXPORT int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_AddConstraint___(void * jarg1, char * jarg2, double jarg3, double jarg4) {
  int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  char *arg2 = (char *) 0 ;
  double arg3 ;
  double arg4 ;
  int result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (char *)jarg2; 
  arg3 = (double)jarg3; 
  arg4 = (double)jarg4; 
  result = (int)(arg1)->AddConstraint((char const *)arg2,arg3,arg4);
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetCoefficient__SWIG_0___(void * jarg1, int jarg2, int jarg3, double jarg4) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  int arg2 ;
  int arg3 ;
  double arg4 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (int)jarg2; 
  arg3 = (int)jarg3; 
  arg4 = (double)jarg4; 
  (arg1)->SetCoefficient(arg2,arg3,arg4);
}


SWIGEXPORT unsigned int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetCoefficient__SWIG_1___(void * jarg1, char * jarg2, char * jarg3, double jarg4) {
  unsigned int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  char *arg2 = (char *) 0 ;
  char *arg3 = (char *) 0 ;
  double arg4 ;
  bool result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (char *)jarg2; 
  arg3 = (char *)jarg3; 
  arg4 = (double)jarg4; 
  result = (bool)(arg1)->SetCoefficient((char const *)arg2,(char const *)arg3,arg4);
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetObjective__SWIG_0___(void * jarg1, int jarg2, double jarg3) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  int arg2 ;
  double arg3 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (int)jarg2; 
  arg3 = (double)jarg3; 
  (arg1)->SetObjective(arg2,arg3);
}


SWIGEXPORT unsigned int SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_SetObjective__SWIG_1___(void * jarg1, char * jarg2, double jarg3) {
  unsigned int jresult ;
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  char *arg2 = (char *) 0 ;
  double arg3 ;
  bool result;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  arg2 = (char *)jarg2; 
  arg3 = (double)jarg3; 
  result = (bool)(arg1)->SetObjective((char const *)arg2,arg3);
  jresult = result; 
  return jresult;
}


SWIGEXPORT void SWIGSTDCALL CSharp_CoinOrfClp_ClpInterface_LoadModel___(void * jarg1) {
  coinwrap::ClpInterface *arg1 = (coinwrap::ClpInterface *) 0 ;
  
  arg1 = (coinwrap::ClpInterface *)jarg1; 
  (arg1)->LoadModel();
}


#ifdef __cplusplus
}
#endif

