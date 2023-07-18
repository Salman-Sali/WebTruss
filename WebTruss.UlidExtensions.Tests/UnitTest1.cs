using FluentAssertions;

namespace WebTruss.UlidExtensions.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void ulid_inverse_should_return_inverse_of_the_ulid()
        {
            var ulid = Ulid.Parse("01H5M9J9BM30QWKHQGPY01AHPC");
            var inverseUlid = Ulid.Parse("7YETBPDPMBWZ83CE8F91ZYNE9K");
            inverseUlid.Should().BeEquivalentTo(ulid.Inverse());
        }
    }
}